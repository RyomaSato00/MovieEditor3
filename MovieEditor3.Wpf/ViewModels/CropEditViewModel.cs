using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Reactive.Bindings;

namespace MovieEditor3.Wpf.ViewModels;

internal partial class CropEditViewModel : ObservableObject
{
    public ReactivePropertySlim<bool> IsCropEnabled { get; } = new(false);

    [ObservableProperty] private ICropProperty? _property = null;

    [RelayCommand]
    private void ResetCropArea()
    {

    }

    public void LoadCropEditInfo(ICropProperty property)
    {
        Property = property;
    }
}
