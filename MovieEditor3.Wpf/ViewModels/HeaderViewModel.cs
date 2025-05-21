using System.Diagnostics;
using System.IO;
using System.Reactive;
using System.Reactive.Subjects;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using MovieEditor3.Wpf.Programs;

namespace MovieEditor3.Wpf.ViewModels;

internal partial class HeaderViewModel(UserSetting userSetting) : ObservableObject
{
    /// <summary>
    /// エクスプローラーの実行ファイル名
    /// </summary>
    private const string EXPLORER_EXE = "EXPLORER.EXE";

    /// <summary>
    /// 出力先ディレクトリのパス
    /// </summary>
    public string OutputDirectory { get; set; } = userSetting.OutputDirectory;

    /// <summary>
    /// 設定画面を開くリクエストを通知するObservable
    /// </summary>
    public IObservable<Unit> OpenSettingRequested => _openSettingRequested;

    /// <summary>
    /// エクスプローラーで出力先ディレクトリを開きます
    /// </summary>
    /// <remarks>
    /// 出力先ディレクトリが存在しない場合は処理を中断します
    /// </remarks>
    [RelayCommand]
    private void OpenExplorer()
    {
        // 出力先ディレクトリが存在しない場合は処理を中断
        if (false == Directory.Exists(OutputDirectory)) return;

        // 出力先ディレクトリを開く
        var info = new ProcessStartInfo(EXPLORER_EXE)
        {
            Arguments = $"\"{OutputDirectory}\"",
            UseShellExecute = true,
        };

        using var process = new Process { StartInfo = info };
        process.Start();
        _ = process.WaitForExitAsync();
    }

    /// <summary>
    /// 設定画面を開くリクエストを発行します
    /// </summary>
    [RelayCommand]
    private void OpenSetting()
    {
        _openSettingRequested.OnNext(Unit.Default);
    }

    /// <summary>
    /// 設定画面を開くリクエストを通知するSubject
    /// </summary>
    private readonly Subject<Unit> _openSettingRequested = new();
}
