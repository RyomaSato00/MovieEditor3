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
    /// クロップ領域のセットアップリクエスト
    /// </summary>
    [ObservableProperty] private SetupCropAreaRequest? _setupCropAreaReq = null;

    /// <summary>
    /// アイテム別のクロップ情報を提供するプロパティ
    /// </summary>
    private ICropProperty? _property = null;

    /// <summary>
    /// クロップ領域変更通知用のサブジェクト
    /// </summary>
    private readonly Subject<Rect> _cropAreaChanged = new();

    /// <summary>
    /// キャンバスサイズ変更通知用のサブジェクト
    /// </summary>
    private readonly Subject<Rect> _canvasSizeChanged = new();

    /// <summary>
    /// キャンバスのサイズ情報
    /// </summary>
    private Rect _canvasSize = Rect.Empty;

    public CropAreaViewModel()
    {
        _cropAreaChanged.Subscribe(UpdateCropArea);
        _canvasSizeChanged.Subscribe(size => _canvasSize = size);

        SetupCropAreaReq = new SetupCropAreaRequest { CropAreaChanged = _cropAreaChanged, CanvasSizeChanged = _canvasSizeChanged };
    }

    /// <summary>
    /// クロップ領域情報を読み込みます
    /// </summary>
    /// <param name="property">アイテム別のクロップ情報を提供するプロパティ</param>
    public void LoadCropAreaInfo(ICropProperty property)
    {
        _property = property;
    }

    /// <summary>
    /// クロップ領域を更新します
    /// </summary>
    /// <param name="area">更新するクロップ領域</param>
    private void UpdateCropArea(Rect area)
    {
        if (_property is not null)
        {
            _property.CropRect = AreaSizeToMovieSize(area);
        }
    }

    /// <summary>
    /// UI上のクロップ領域サイズを実際の動画サイズに変換します
    /// </summary>
    /// <param name="areaSize">UI上のクロップ領域サイズ</param>
    /// <returns>実際の動画サイズに変換されたクロップ領域</returns>
    private Rect AreaSizeToMovieSize(Rect areaSize)
    {
        var result = Rect.Empty;

        if (_property is ICropProperty info
        && _canvasSize != Rect.Empty
        && _canvasSize.Width != 0
        && _canvasSize.Height != 0)
        {
            result = new Rect
            {
                X = areaSize.X * info.OriginalWidth / _canvasSize.Width,
                Y = areaSize.Y * info.OriginalHeight / _canvasSize.Height,
                Width = areaSize.Width * info.OriginalWidth / _canvasSize.Width,
                Height = areaSize.Height * info.OriginalHeight / _canvasSize.Height,
            };
        }

        return result;
    }
}
