using System.Windows;

using Microsoft.Xaml.Behaviors;

using MovieEditor3.Wpf.Views;

namespace MovieEditor3.Wpf.Messengers;

internal class SetupCropAreaAction : TriggerAction<CropArea>
{
    protected override void Invoke(object parameter)
    {
        if (parameter is DependencyPropertyChangedEventArgs e
        && e.NewValue is SetupCropAreaRequest request)
        {
            AssociatedObject.Setup(request.CropAreaChanged);
        }
    }
}

internal class ClearCropAreaAction : TriggerAction<CropArea>
{
    protected override void Invoke(object parameter)
    {
        if (parameter is DependencyPropertyChangedEventArgs e
        && e.NewValue is CropActionRequest)
        {
            AssociatedObject.ClearCropArea();
        }
    }
}

internal class SetCropAreaAction : TriggerAction<CropArea>
{
    protected override void Invoke(object parameter)
    {
        if (parameter is DependencyPropertyChangedEventArgs e
        && e.NewValue is SetCropAreaRequest request)
        {
            AssociatedObject.SetCropArea(request.RelativeCropArea);
        }
    }
}

internal class SetupCropAreaRequest
{
    public required IObserver<Rect> CropAreaChanged { get; init; }
}

internal class CropActionRequest { }

internal class SetCropAreaRequest
{
    public required Rect RelativeCropArea { get; init; }
}
