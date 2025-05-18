namespace MovieEditor3.Wpf.Programs;

/// <summary>
/// 画像生成処理の入力情報を定義するインターフェース
/// </summary>
internal interface IGenerateImagesInfo
{
    /// <summary>
    /// メディアファイルのパスを取得します
    /// </summary>
    public string FilePath { get; }

    /// <summary>
    /// 1秒あたりのフレーム数を取得します
    /// </summary>
    public int FramesPerSecond { get; }

    /// <summary>
    /// 生成するフレームの総数を取得します
    /// </summary>
    public int CountOfFrames { get; }

    /// <summary>
    /// 生成する画像の品質を取得します
    /// </summary>
    public int Quality { get; }

    /// <summary>
    /// 画像生成の開始ポイントを取得します
    /// </summary>
    public TimeSpan? StartPoint { get; }

    /// <summary>
    /// 画像生成の終了ポイントを取得します
    /// </summary>
    public TimeSpan? EndPoint { get; }

    /// <summary>
    /// 出力先ディレクトリを取得します
    /// </summary>
    public string OutputDirectory { get; }

    /// <summary>
    /// 出力ファイル名のベース名を取得します
    /// </summary>
    public string OutputName { get; }
}
