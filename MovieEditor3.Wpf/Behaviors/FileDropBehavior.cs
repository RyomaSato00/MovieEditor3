using System.Reactive.Subjects;
using System.Windows;

using Microsoft.Xaml.Behaviors;

namespace MovieEditor3.Wpf.Behaviors;

public class FileDropBehavior : Behavior<UIElement>
{
    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.AllowDrop = true;
        AssociatedObject.PreviewDragOver += OnDragOver;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject.PreviewDragOver -= OnDragOver;
    }

    private void OnDragOver(object sender, DragEventArgs e)
    {
        // ドラッグされているデータがファイルである場合のみコピーエフェクトを表示
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            e.Effects = DragDropEffects.Copy;
        }
        else
        {
            e.Effects = DragDropEffects.None;
        }
        e.Handled = true;
    }
}
