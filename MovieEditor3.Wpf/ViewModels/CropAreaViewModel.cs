using System.Reactive.Subjects;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using MovieEditor3.Wpf.Messengers;

using Reactive.Bindings;

namespace MovieEditor3.Wpf.ViewModels;

/// <summary>
/// 動画のクロップ（切り抜き）領域を管理するビューモデルクラス
/// </summary>
internal partial class CropAreaViewModel : ObservableObject
{
    /// <summary>
    /// アイテム別のクロップ情報を提供するプロパティ
    /// </summary>
    private ICropProperty? _property = null;

    /// <summary>
    /// クロップ領域変更通知用のサブジェクト
    /// </summary>
    private readonly Subject<Rect> _cropAreaChanged = new();

    [ObservableProperty] private SetupCropAreaRequest? _setupCropAreaReq = null;
    [ObservableProperty] private CropActionRequest? _clearCropAreaReq = null;
    [ObservableProperty] private SetCropAreaRequest? _setCropAreaReq = null;


    public CropAreaViewModel()
    {
        _cropAreaChanged.Subscribe(UpdateCropArea);

        SetupCropAreaReq = new SetupCropAreaRequest { CropAreaChanged = _cropAreaChanged };
    }

    /// <summary>
    /// クロップ領域情報を読み込みます
    /// </summary>
    /// <param name="property">アイテム別のクロップ情報を提供するプロパティ</param>
    public void LoadCropAreaInfo(ICropProperty? property)
    {
        _property = property;
        if (property is null || Rect.Empty == property.CropRect)
        {
            ClearCropArea();
        }
        else
        {
            SetCropArea(property.CropRect);
        }
    }

    /// <summary>
    /// クロップ領域を更新します
    /// </summary>
    /// <param name="relativeRect">更新するクロップ領域</param>
    private void UpdateCropArea(Rect relativeRect)
    {
        if (_property is not null)
        {
            _property.CropRect = RelativeToMovieSize(relativeRect);
        }
    }

    /// <summary>
    /// 相対座標系のクロップ領域を実際の動画サイズの座標系に変換します
    /// </summary>
    /// <param name="relativeRect">相対座標系（0.0～1.0の範囲）のクロップ領域</param>
    /// <returns>実際の動画サイズに変換されたクロップ領域</returns>
    private Rect RelativeToMovieSize(Rect relativeRect)
    {
        var result = Rect.Empty;

        if (_property is ICropProperty info)
        {
            result = new Rect
            {
                X = relativeRect.X * info.OriginalWidth,
                Y = relativeRect.Y * info.OriginalHeight,
                Width = relativeRect.Width * info.OriginalWidth,
                Height = relativeRect.Height * info.OriginalHeight,
            };
        }

        return result;
    }

    /// <summary>
    /// 実際の動画サイズの座標系のクロップ領域を相対座標系に変換します
    /// </summary>
    /// <param name="movieSizeRect">実際の動画サイズの座標系のクロップ領域</param>
    /// <returns>相対座標系（0.0～1.0の範囲）に変換されたクロップ領域</returns>
    private Rect MovieSizeToRelative(Rect movieSizeRect)
    {
        var result = Rect.Empty;

        if (_property is ICropProperty info
        && 0 != info.OriginalWidth && 0 != info.OriginalHeight)
        {
            result = new Rect
            {
                X = movieSizeRect.X / info.OriginalWidth,
                Y = movieSizeRect.Y / info.OriginalHeight,
                Width = movieSizeRect.Width / info.OriginalWidth,
                Height = movieSizeRect.Height / info.OriginalHeight,
            };
        }

        return result;
    }

    /// <summary>
    /// クロップ領域をクリアします
    /// </summary>
    public void ClearCropArea() => ClearCropAreaReq = new CropActionRequest();

    /// <summary>
    /// クロップ領域を設定するトリガーアクション
    /// </summary>
    private void SetCropArea(Rect movieSizeRect)
    {
        var relativeRect = MovieSizeToRelative(movieSizeRect);
        SetCropAreaReq = new SetCropAreaRequest { RelativeCropArea = relativeRect };
    }
}
