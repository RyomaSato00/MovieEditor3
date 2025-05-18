using System.Windows;

namespace MovieEditor3.Wpf.ViewModels;

/// <summary>
/// 動画のクロップ（切り抜き）ビューにおいてアクセス可能なプロパティを定義するインターフェース
/// </summary>
internal interface ICropProperty
{
    /// <summary>
    /// クロップ領域の矩形情報を取得または設定します
    /// </summary>
    public Rect CropRect { get; set; }

    /// <summary>
    /// 元の動画の幅を取得します
    /// </summary>
    public int OriginalWidth { get; }

    /// <summary>
    /// 元の動画の高さを取得します
    /// </summary>
    public int OriginalHeight { get; }
}
