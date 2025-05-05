using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Reactive.Bindings;

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
    public MediaListViewModel MediaListViewContext { get; } = new();

    /// <summary>
    /// コンテンツ表示状態
    /// </summary>
    [ObservableProperty] private bool _isContentVisible = false;

    public MainWindowViewModel()
    {
        // イベント設定
        EmptyStateViewContext.OnNewFilesAdded.Subscribe(files => _ = MediaListViewContext.AddItemsAsync(files));
        MediaListViewContext.IsEmpty.Subscribe(isEmpty => EmptyStateViewContext.IsVisible = isEmpty);
        MediaListViewContext.IsEmpty.Subscribe(isEmpty => IsContentVisible = false == isEmpty);
    }

}
