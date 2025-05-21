using System.Reactive;
using System.Windows;

using Microsoft.Xaml.Behaviors;

using MovieEditor3.Wpf.ViewModels;

namespace MovieEditor3.Wpf.Messengers;

internal class ShowCommandCheckDialogAction : TriggerAction<MainWindow>
{
    protected override void Invoke(object parameter)
    {
        if (parameter is DependencyPropertyChangedEventArgs e
        && e.NewValue is ShowDialogRequest)
        {
            AssociatedObject.ShowCommandCheckDialog();
        }
    }
}

internal class ShowProgressDialogAction : TriggerAction<MainWindow>
{
    protected override void Invoke(object parameter)
    {
        if (parameter is DependencyPropertyChangedEventArgs e
        && e.NewValue is ShowDialogRequest)
        {
            AssociatedObject.ShowProgressDialog();
        }
    }
}

internal class ShowDeleteDialogAction : TriggerAction<MainWindow>
{
    protected override void Invoke(object parameter)
    {
        if (parameter is DependencyPropertyChangedEventArgs e
        && e.NewValue is ShowDialogRequest)
        {
            AssociatedObject.ShowDeleteDialog();
        }
    }
}

internal class ShowSettingDialogAction : TriggerAction<MainWindow>
{
    protected override void Invoke(object parameter)
    {
        if (parameter is DependencyPropertyChangedEventArgs e
        && e.NewValue is ShowDialogRequest)
        {
            AssociatedObject.ShowSettingDialog();
        }
    }
}

internal class ShowDialogRequest {}
