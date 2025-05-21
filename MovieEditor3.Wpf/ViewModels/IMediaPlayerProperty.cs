namespace MovieEditor3.Wpf.ViewModels;

/// <summary>
/// メディアプレーヤーの基本プロパティを定義するインターフェース
/// </summary>
/// <remarks>
/// このインターフェースはICropPropertyを継承し、メディアプレイヤーの表示に必要な
/// 基本的なプロパティを提供します。
/// </remarks>
internal interface IMediaPlayerProperty : ICropProperty
{
    /// <summary>
    /// メディアファイルのパス
    /// </summary>
    public string FilePath { get; }

    /// <summary>
    /// 編集の開始ポイント
    /// </summary>
    public TimeSpan? StartPoint { get; }

    /// <summary>
    /// 編集の終了ポイント
    /// </summary>
    public TimeSpan? EndPoint { get; }
}
