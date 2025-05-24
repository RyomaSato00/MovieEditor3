using System.Diagnostics;
using System.IO;
using System.Reactive;
using System.Reactive.Subjects;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using MovieEditor3.Wpf.Programs;

namespace MovieEditor3.Wpf.ViewModels;

internal partial class HeaderViewModel : ObservableObject
{
    /// <summary>
    /// ファイル結合リクエストを通知するObservable
    /// </summary>
    public IObservable<Unit> JoinFilesRequested => _joinFilesRequested;

    /// <summary>
    /// エクスプローラーで出力先ディレクトリを開くリクエストを通知するObservable
    /// </summary>
    public IObservable<Unit> OpenExplorerRequested => _openExplorerRequested;

    /// <summary>
    /// 設定画面を開くリクエストを通知するObservable
    /// </summary>
    public IObservable<Unit> OpenSettingRequested => _openSettingRequested;

    /// <summary>
    /// ファイル結合処理を実行するリクエストを発行します
    /// </summary>
    [RelayCommand]
    private void JoinFiles()
    {
        _joinFilesRequested.OnNext(Unit.Default);
    }

    /// <summary>
    /// エクスプローラーで出力先ディレクトリを開くリクエストを発行します
    /// </summary>
    [RelayCommand]
    private void OpenExplorer()
    {
        _openExplorerRequested.OnNext(Unit.Default);
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
    /// ファイル結合リクエストを通知するSubject
    /// </summary>
    private readonly Subject<Unit> _joinFilesRequested = new();

    /// <summary>
    /// エクスプローラで出力先ディレクトリを開くリクエストを通知するSubject
    /// </summary>
    /// <returns></returns>
    private readonly Subject<Unit> _openExplorerRequested = new();

    /// <summary>
    /// 設定画面を開くリクエストを通知するSubject
    /// </summary>
    private readonly Subject<Unit> _openSettingRequested = new();
}
