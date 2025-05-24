using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using FFMpegCore;

using MovieEditor3.Wpf.Messengers;
using MovieEditor3.Wpf.Programs;
using MovieEditor3.Wpf.Views;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace MovieEditor3.Wpf.ViewModels;

internal partial class MainWindowViewModel : ObservableObject
{
    /// <summary>
    /// メディアリスト用ViewModel
    /// </summary>
    public MediaListViewModel MediaListViewContext { get; }

    /// <summary>
    /// 編集エリア用ViewModel
    /// </summary>
    public EditViewModel EditViewContext { get; } = new();

    /// <summary>
    /// ヘッダービュー用ViewModel
    /// </summary>
    /// <returns></returns>
    public HeaderViewModel HeaderViewContext { get; }

    /// <summary>
    /// フッタービュー用ViewModel
    /// </summary>
    public FooterViewModel FooterViewContext { get; } = new();

    /// <summary>
    /// 設定ダイアログ用ViewModel
    /// </summary>
    public SettingDialogViewModel SettingDialogContext { get; }

    /// <summary>
    /// コマンド確認ダイアログ用ViewModel
    /// </summary>
    public CommandCheckDialogViewModel CommandCheckDialogContext { get; } = new();

    /// <summary>
    /// 進捗ダイアログ用ViewModel
    /// </summary
    public ProgressDialogViewModel ProgressDialogContext { get; } = new();

    /// <summary>
    /// 削除確認ダイアログ用ViewModel
    /// </summary>
    public YesNoDialogViewModel DeleteDialogContext { get; } = new YesNoDialogViewModel { Message = "処理済みファイルをリストから削除しますか？" };

    [ObservableProperty] private ShowDialogRequest? _showCommandCheckDialogReq = null;
    [ObservableProperty] private ShowDialogRequest? _showProgressDialogReq = null;
    [ObservableProperty] private ShowDialogRequest? _showDeleteDialogReq = null;
    [ObservableProperty] private ShowDialogRequest? _showSettingDialogReq = null;

    public MainWindowViewModel(UserSetting userSetting)
    {
        MediaListViewContext = new MediaListViewModel(userSetting);
        HeaderViewContext = new HeaderViewModel(userSetting);
        SettingDialogContext = new SettingDialogViewModel(userSetting);

        // 各種イベント設定
        MediaListViewContext.IsEmpty.Where(isEmpty => false == isEmpty).Subscribe(isEmpty => EditViewContext.SelectItem(MediaListViewContext.MediaItems[0]));
        MediaListViewContext.OnSelected.Subscribe(EditViewContext.SelectItem);
        MediaListViewContext.IsAllSelected.Subscribe(isSelected => FooterViewContext.IsExecutionEnabled.Value = isSelected ?? true);
        HeaderViewContext.JoinFilesRequested.Subscribe(unit => _ = JoinFilesAsync());
        HeaderViewContext.OpenSettingRequested.Subscribe(_ => ShowSettingDialogReq = new ShowDialogRequest());
        FooterViewContext.CompCommand.Subscribe(unit => _ = CompAsync());
        FooterViewContext.GenerateImagesCommand.Subscribe(unit => _ = GenerateImagesAsync());
    }

    /// <summary>
    /// ユーザー設定を保存します
    /// </summary>
    /// <param name="userSetting">保存先のユーザー設定オブジェクト</param>
    public void SaveSetting(UserSetting userSetting)
    {
        UserSetting.Copy(SettingDialogContext.UserSettingBackup, userSetting);
    }

    /// <summary>
    /// 選択された動画ファイルの圧縮処理を実行します。
    /// </summary>
    /// <returns>処理の実行タスク</returns>
    /// <remarks>
    /// 処理の流れ:
    /// 1. 選択された各メディアアイテムに対して圧縮用のFFmpegコマンドを生成
    /// 2. コマンド確認ダイアログを表示し、ユーザーに実行前の確認を求める
    /// 3. 確認後、進捗ダイアログを表示し、圧縮コマンドを並列実行
    /// 4. 処理完了後、処理済みファイルをリストから削除するかユーザーに確認
    /// 5. ユーザーの選択に応じて、処理済みファイルをリストから削除
    ///
    /// 圧縮処理では各動画ファイルに対して、設定された圧縮パラメータが適用されます。
    /// ユーザーは処理の各段階で操作をキャンセルすることができます。
    /// </remarks>
    private async Task CompAsync()
    {
        var commandCheckInfos = MediaListViewContext.MediaItems
        .Where(static item => item.IsSelected)
        .Select(item => new CommandCheckInfo
        (
            FFmpegCommandConverter.ToCompressCommand(item, SettingDialogContext.UserSettingBackup.OutputDirectory),
            item.ThumbnailPath
        ))
        .ToArray();

        CommandCheckDialogContext.CommandProperties = commandCheckInfos;

        // コマンドチェックダイアログを表示
        ShowCommandCheckDialogReq = new ShowDialogRequest();

        var isChecked = await CommandCheckDialogContext.WaitForCommandChecked();

        if (isChecked == false) return;

        var processItems = commandCheckInfos
        .Select(static info => new CommandProcessInfo(info.Command))
        .ToArray();

        ProgressDialogContext.SetProgress(processItems);

        // 処理進捗ダイアログを表示
        ShowProgressDialogReq = new ShowDialogRequest();

        await Task.Run(() => ParallelCommandProcessor.Run(processItems));

        ProgressDialogContext.Close();

        // 削除ダイアログを表示
        ShowDeleteDialogReq = new ShowDialogRequest();

        var isDeleteRequested = await DeleteDialogContext.WaitForAnswer();

        if (isDeleteRequested)
        {
            MediaListViewContext.DeleteSelectedItems();
        }
    }

    /// <summary>
    /// 選択された複数の動画ファイルを結合する処理を実行します。
    /// </summary>
    /// <remarks>
    /// 処理の流れ:
    /// 1. 選択された各メディアアイテムに対して結合用の前処理コマンドを生成
    /// 2. 進捗ダイアログを表示し、前処理コマンドを並列実行
    /// 3. 前処理で生成された一時ファイルを使用して、FFMpegのJoin機能で動画を結合
    /// 4. 結合された動画ファイルをメディアリストに追加
    ///
    /// 前処理では各動画ファイルのトリミングや編集が適用され、結合に適した形式に変換されます。
    /// 結合後のファイルは一意のGUIDを含むファイル名で保存されます。
    /// </remarks>
    private async Task JoinFilesAsync()
    {
        var processItems = MediaListViewContext.SelectedItems
        .Select(static item => new CommandProcessInfo(FFmpegCommandConverter.ToArrangeCommandForJoin(item)))
        .ToArray();

        ProgressDialogContext.SetProgress(processItems);

        // 処理進捗ダイアログを表示
        ShowProgressDialogReq = new ShowDialogRequest();

        await Task.Run(() => ParallelCommandProcessor.Run(processItems));

        ProgressDialogContext.Close();

        // 各結合素材ファイルのパスを取得
        var outputFiles = processItems
        .Select(static item => FFmpegCommandConverter.GetOutputFilePathFromCommand(item.Command))
        .Where(static item => File.Exists(item))
        .ToArray();

        // 動画結合出力ファイルパスを作成
        var joinedFilePath = FFmpegCommandConverter.NewJoinedFilePath();

        // 動画結合処理を実行
        FFMpeg.Join(joinedFilePath, outputFiles);

        // 結合後の動画をリストに追加
        _ = MediaListViewContext.AddItemAsync(joinedFilePath);
    }

    /// <summary>
    /// 選択された動画ファイルから画像を生成する処理を実行します。
    /// </summary>
    /// <returns>処理の実行タスク</returns>
    /// <remarks>
    /// 処理の流れ:
    /// 1. 選択された各メディアアイテムに対して画像生成用のFFmpegコマンドを生成
    /// 2. コマンド確認ダイアログを表示し、ユーザーに実行前の確認を求める
    /// 3. 確認後、進捗ダイアログを表示し、画像生成コマンドを並列実行
    /// 4. 処理完了後、処理済みファイルをリストから削除するかユーザーに確認
    /// 5. ユーザーの選択に応じて、処理済みファイルをリストから削除
    ///
    /// 画像生成処理では各動画ファイルに対して、設定されたフレーム数、品質、
    /// 抽出範囲などのパラメータが適用されます。
    /// 生成された画像は指定された出力ディレクトリに保存されます。
    /// ユーザーは処理の各段階で操作をキャンセルすることができます。
    /// </remarks>
    private async Task GenerateImagesAsync()
    {
        var commandCheckInfos = MediaListViewContext.MediaItems
        .Where(static item => item.IsSelected)
        .Select(item => new CommandCheckInfo
        (
            FFmpegCommandConverter.ToImagesCommand(item, SettingDialogContext.UserSettingBackup.OutputDirectory),
            item.ThumbnailPath
        ))
        .ToArray();

        CommandCheckDialogContext.CommandProperties = commandCheckInfos;

        // コマンドチェックダイアログを表示
        ShowCommandCheckDialogReq = new ShowDialogRequest();

        var isChecked = await CommandCheckDialogContext.WaitForCommandChecked();

        if (isChecked == false) return;

        var processItems = commandCheckInfos
        .Select(static info => new CommandProcessInfo(info.Command))
        .ToArray();

        ProgressDialogContext.SetProgress(processItems);

        // 処理進捗ダイアログを表示
        ShowProgressDialogReq = new ShowDialogRequest();

        await Task.Run(() => ParallelCommandProcessor.Run(processItems));

        ProgressDialogContext.Close();

        // 削除ダイアログを表示
        ShowDeleteDialogReq = new ShowDialogRequest();

        var isDeleteRequested = await DeleteDialogContext.WaitForAnswer();

        if (isDeleteRequested)
        {
            MediaListViewContext.DeleteSelectedItems();
        }
    }

}
