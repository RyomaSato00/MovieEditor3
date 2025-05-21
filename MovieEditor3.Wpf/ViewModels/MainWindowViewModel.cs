using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using MovieEditor3.Wpf.Messengers;
using MovieEditor3.Wpf.Programs;
using MovieEditor3.Wpf.Views;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace MovieEditor3.Wpf.ViewModels;

internal partial class MainWindowViewModel : ObservableObject
{
    /// <summary>
    /// エンプティステート用ViewModel
    /// </summary>
    public EmptyStateViewModel EmptyStateViewContext { get; } = new();

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
    /// 実行操作用ViewModel
    /// </summary>
    public ExecutionViewModel ExecutionViewContext { get; } = new();

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

    /// <summary>
    /// コンテンツ表示状態
    /// </summary>
    [ObservableProperty] private bool _isContentVisible = false;

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
        EmptyStateViewContext.OnNewFilesAdded.Subscribe(files => _ = MediaListViewContext.AddItemsAsync(files));
        MediaListViewContext.IsEmpty.Subscribe(isEmpty => EmptyStateViewContext.IsVisible = isEmpty);
        MediaListViewContext.IsEmpty.Subscribe(isEmpty => IsContentVisible = false == isEmpty);
        MediaListViewContext.OnSelected.Subscribe(EditViewContext.SelectItem);
        MediaListViewContext.IsAllSelected.Subscribe(isSelected => ExecutionViewContext.IsExecutionEnabled.Value = isSelected ?? true);
        HeaderViewContext.OpenSettingRequested.Subscribe(_ => ShowSettingDialogReq = new ShowDialogRequest());
        ExecutionViewContext.CompCommand.Subscribe(unit => _ = Comp());
    }

/// <summary>
    /// ユーザー設定を保存します
    /// </summary>
    /// <param name="userSetting">保存先のユーザー設定オブジェクト</param>
    public void SaveSetting(UserSetting userSetting)
    {
        UserSetting.Copy(SettingDialogContext.UserSettingBackup,  userSetting);
    }

    /// <summary>
    /// 圧縮処理を実行します
    /// </summary>
    /// <returns>処理の実行タスク</returns>
    private async Task Comp()
    {
        var commandCheckInfos = MediaListViewContext.MediaItems
        .Where(static item => item.IsSelected)
        .Select(static item => new CommandCheckInfo
        (
            FFmpegCommandConverter.ToCompressCommand(item),
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
