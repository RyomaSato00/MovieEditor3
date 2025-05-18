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
            AssociatedObject.Setup(request.CropAreaChanged, request.CanvasSizeChanged);
        }
    }
}

internal class SetupCropAreaRequest
{
    public required IObserver<Rect> CropAreaChanged { get; init; }
    public required IObserver<Rect> CanvasSizeChanged { get; init; }
}
