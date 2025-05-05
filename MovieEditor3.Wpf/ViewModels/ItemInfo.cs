using CommunityToolkit.Mvvm.ComponentModel;

using MovieEditor3.Wpf.Programs;

namespace MovieEditor3.Wpf.ViewModels;

/// <summary>
/// メディアアイテムの情報を管理するクラス
/// </summary>
internal partial class ItemInfo : ObservableObject
{
    /// <summary>
    /// 選択状態
    /// </summary>
    [ObservableProperty] private bool _isSelected = false;

    /// <summary>
    /// サムネイル画像のパス
    /// </summary>
    [ObservableProperty] private string _thumbnailPath = string.Empty;

    /// <summary>
    /// オリジナルメディア情報
    /// </summary>
    public MediaInfo OriginalMediaInfo { get; private set; } = MediaInfo.Empty;

    /// <summary>
    /// 指定したファイルパスからメディア情報とサムネイルを読み込む
    /// </summary>
    /// <param name="filePath">メディアファイルのパス</param>
    public void LoadInfo(string filePath)
    {
        // メディア情報を読み込む
        OriginalMediaInfo = MediaInfo.ToMediaInfo(filePath);

        // サムネイル画像を生成し、パスを設定
        ThumbnailPath = ThumbnailCreator.CreateThumbnail(filePath, TimeSpan.Zero);
    }

    /// <summary>
    /// 他のItemInfoからメディア情報をコピーする
    /// </summary>
    /// <param name="itemInfo">コピー元のメディア情報</param>
    public void CopyInfoFrom(ItemInfo itemInfo)
    {
        // サムネイル画像のパスをコピー
        ThumbnailPath = itemInfo.ThumbnailPath;

        // メディア情報をコピー（ディープコピー）
        OriginalMediaInfo = itemInfo.OriginalMediaInfo with {};
    }
}
