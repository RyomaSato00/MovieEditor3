using System.Windows;

using Microsoft.Xaml.Behaviors;

using MovieEditor3.Wpf.Views;

namespace MovieEditor3.Wpf.Messengers;

internal class ShowWaitingDialogAction : TriggerAction<MediaListView>
{
    protected override void Invoke(object parameter)
    {
        if (parameter is DependencyPropertyChangedEventArgs e
        && e.NewValue is ShowDialogRequest)
        {
            AssociatedObject.ShowWaitingDialog();
        }
    }
}
