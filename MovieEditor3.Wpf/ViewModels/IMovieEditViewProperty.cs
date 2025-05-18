namespace MovieEditor3.Wpf.ViewModels;

/// <summary>
/// 動画編集ビューにおいてアクセス可能なプロパティを定義するインターフェース
/// </summary>
internal interface IMovieEditViewProperty
{
    /// <summary>
    /// メディアファイルのパスを取得します
    /// </summary>
    public string FilePath { get; }

    /// <summary>
    /// メディアのデフォルト再生時間を取得します
    /// </summary>
    public TimeSpan DefaultDuration { get; }

    /// <summary>
    /// 編集の開始ポイントを取得または設定します
    /// </summary>
    public TimeSpan? StartPoint { get; set; }

    /// <summary>
    /// 編集の終了ポイントを取得または設定します
    /// </summary>
    public TimeSpan? EndPoint { get; set; }

    /// <summary>
    /// 開始ポイントのサムネイル画像パスを取得または設定します
    /// </summary>
    public string StartPointImagePath { get; set; }

    /// <summary>
    /// 終了ポイントのサムネイル画像パスを取得または設定します
    /// </summary>
    public string EndPointImagePath { get; set; }

}
