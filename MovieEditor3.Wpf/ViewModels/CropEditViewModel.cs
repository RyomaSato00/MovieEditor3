using System.Reactive;
using System.Reactive.Subjects;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using MovieEditor3.Wpf.Programs;

namespace MovieEditor3.Wpf.ViewModels;

/// <summary>
/// クロップ（切り抜き）編集機能のビューモデルクラス
/// </summary>
internal partial class CropEditViewModel : ObservableObject
{
    private const int RIGHT_ROTATE = 1;

    private const int LEFT_ROTATE = -1;

    /// <summary>
    /// クロップ領域のクリアが要求されたことを通知するObservable
    /// </summary>
    public IObservable<Unit> OnClearCropAreaRequested => _onClearCropAreaRequested;

    /// <summary>
    /// 動画の回転が要求されたことを通知するObservable
    /// </summary>
    public IObservable<int> OnRotateRequested => _onRotateRequested;

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
    /// 動画を右に90度回転します
    /// </summary>
    [RelayCommand]
    private void RightRotate()
    {
        _onRotateRequested.OnNext(RIGHT_ROTATE);

        if (Property is not null)
        {
            Property.Rotation = (RotateID)(((int)Property.Rotation + 1) % (int)RotateID.Count);
        }
    }

    /// <summary>
    /// 動画を左に90度回転します
    /// </summary>
    [RelayCommand]
    private void LeftRotate()
    {
        _onRotateRequested.OnNext(LEFT_ROTATE);

        if (Property is not null)
        {
            Property.Rotation = ((int)Property.Rotation - 1) >= 0 ? (RotateID)((int)Property.Rotation - 1) : RotateID.Rotate270;
        }
    }

    /// <summary>
    /// クロップ領域のクリアが要求されたことを通知するSubject
    /// </summary>
    private readonly Subject<Unit> _onClearCropAreaRequested = new();

    /// <summary>
    /// 動画の回転が要求されたことを通知するSubject
    /// </summary>
    /// <returns></returns>
    private readonly Subject<int> _onRotateRequested = new();

    /// <summary>
    /// クロップ編集情報を読み込みます
    /// </summary>
    /// <param name="property">クロップ情報を提供するプロパティ</param>
    public void LoadCropEditInfo(ICropProperty? property)
    {
        Property = property;
    }
}
