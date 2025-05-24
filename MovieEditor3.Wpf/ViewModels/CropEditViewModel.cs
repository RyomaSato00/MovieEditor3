using System.Reactive;
using System.Reactive.Subjects;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Reactive.Bindings;

namespace MovieEditor3.Wpf.ViewModels;

/// <summary>
/// クロップ（切り抜き）編集機能のビューモデルクラス
/// </summary>
internal partial class CropEditViewModel : ObservableObject
{
    /// <summary>
    /// クロップ領域のクリアが要求されたことを通知するObservable
    /// </summary>
    public IObservable<Unit> OnClearCropAreaRequested => _onClearCropAreaRequested;

    /// <summary>
    /// クロップ情報を提供するプロパティ
    /// </summary>
    [ObservableProperty] private ICropProperty? _property = null;

    /// <summary>
    /// クロップ領域およびクロップ情報をクリアします
    /// </summary>
    [RelayCommand]
    private void ClearCropArea()
    {
        _onClearCropAreaRequested.OnNext(Unit.Default);

        if (Property is not null)
        {
            Property.CropRect = Rect.Empty;
        }
    }

    /// <summary>
    /// クロップ領域のクリアが要求されたことを通知するSubject
    /// </summary>
    private readonly Subject<Unit> _onClearCropAreaRequested = new();

    /// <summary>
    /// クロップ編集情報を読み込みます
    /// </summary>
    /// <param name="property">クロップ情報を提供するプロパティ</param>
    public void LoadCropEditInfo(ICropProperty? property)
    {
        Property = property;
    }
}
