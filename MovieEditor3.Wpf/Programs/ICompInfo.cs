using System.Windows;

namespace MovieEditor3.Wpf.Programs;

/// <summary>
/// 動画の圧縮処理の入力情報を定義するインターフェース
/// </summary>
internal interface ICompInfo
{
    /// <summary>
    /// 元動画の幅を取得します
    /// </summary>
    public int OriginalWidth { get; }

    /// <summary>
    /// 元動画の高さを取得します
    /// </summary>
    public int OriginalHeight { get; }

    /// <summary>
    /// メディアファイルのパスを取得します
    /// </summary>
    public string FilePath { get; }

    /// <summary>
    /// リサイズ後の幅を取得します
    /// </summary>
    public int ResizedWidth { get; }

    /// <summary>
    /// リサイズ後の高さを取得します
    /// </summary>
    public int ResizedHeight { get; }

    /// <summary>
    /// クロップ領域の矩形情報を取得します
    /// </summary>
    public Rect CropRect { get; }

    /// <summary>
    /// 回転の種類を取得します
    /// </summary>
    public RotateID Rotation { get; }

    /// <summary>
    /// 再生速度を取得します
    /// </summary>
    public float PlaybackSpeed { get; }

    /// <summary>
    /// フレームレートを取得します
    /// </summary>
    public float FrameRate { get; }

    /// <summary>
    /// 使用するコーデックを取得します
    /// </summary>
    public string Codec { get; }

    /// <summary>
    /// 音声を無効にするかどうかを取得します
    /// </summary>
    public bool IsAudioDisabled { get; }

    /// <summary>
    /// 編集の開始ポイントを取得します
    /// </summary>
    public TimeSpan? StartPoint { get; }

    /// <summary>
    /// 編集の終了ポイントを取得します
    /// </summary>
    public TimeSpan? EndPoint { get; }

    /// <summary>
    /// 出力ファイル名を取得します
    /// </summary>
    public string GuidName { get; }
}

/// <summary>
/// 回転の種類を定義する列挙型
/// </summary>
internal enum RotateID
{
    /// <summary>
    /// 回転なし
    /// </summary>
    None = 0,

    /// <summary>
    /// 90度回転
    /// </summary>
    Rotate90,

    /// <summary>
    /// 180度回転
    /// </summary>
    Rotate180,

    /// <summary>
    /// 270度回転
    /// </summary>
    Rotate270,

    /// <summary>
    /// 列挙型の要素数
    /// </summary>
    Count
}
