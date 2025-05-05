using System.Reactive.Subjects;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MovieEditor3.Wpf.ViewModels;

internal partial class EmptyStateViewModel : ObservableObject
{
    /// <summary>
    /// エンプティステートの表示/非表示
    /// </summary>
    [ObservableProperty] private bool _isVisible = true;

    /// <summary>
    /// 新しいファイルが追加された通知
    /// </summary>
    public IObservable<string[]> OnNewFilesAdded => _onNewFilesAdded;

    /// <summary>
    /// ドロップイベントハンドラ
    /// </summary>
    /// <param name="e">ドラッグイベント引数</param>
    [RelayCommand]
    private void Drop(DragEventArgs e)
    {
        // ドロップされたデータがファイルではない場合は何もしない
        if (false == e.Data.GetDataPresent(DataFormats.FileDrop)) return;

        // ドロップされたデータをファイルパスの配列として取得
        var files = (string[])e.Data.GetData(DataFormats.FileDrop);

        // 新しいファイルが追加された通知を発行
        _onNewFilesAdded.OnNext(files);
    }

    /// <summary>
    /// 新しいファイル追加通知用のSubject
    /// </summary>
    private readonly Subject<string[]> _onNewFilesAdded = new();
}
