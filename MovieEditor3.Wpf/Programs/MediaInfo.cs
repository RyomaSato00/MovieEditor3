using System.IO;

using FFMpegCore;

namespace MovieEditor3.Wpf.Programs;

internal record MediaInfo
{
    /// <summary> ファイルパス </summary>
    public required string FilePath { get; init; }

    /// <summary> 拡張子なしファイル名 </summary>
    public required string FileNameWithoutExtension { get; init; }

    /// <summary> 拡張子 </summary>
    public required string Extension { get; init; }

    /// <summary> 幅(px) </summary>
    public required int Width { get; init; }

    /// <summary> 高さ(px) </summary>
    public required int Height { get; init; }

    /// <summary> 総再生時間 </summary>
    public required TimeSpan Duration { get; init; }

    /// <summary> フレームレート(fps) </summary>
    public required double FrameRate { get; init; }

    /// <summary> 動画コーデック </summary>
    public required string VideoCodec { get; init; }

    /// <summary> ファイルサイズ(byte) </summary>
    public required long FileSize { get; init; }

    /// <summary> Empty State </summary>
    public static readonly MediaInfo Empty = new()
    {
        FilePath = string.Empty,
        FileNameWithoutExtension = string.Empty,
        Extension = string.Empty,
        Width = -1,
        Height = -1,
        Duration = TimeSpan.Zero,
        FrameRate = -1.0,
        VideoCodec = string.Empty,
        FileSize = -1
    };

    /// <summary> 適用可能拡張子リスト </summary>
    public static readonly IEnumerable<string> AvailableExtensions =
    [
        ".mp4", ".MP4", ".mov", ".MOV", "agm", "AGM", "avi", "AVI", "wmv", "WMV",
    ];

    /// <summary>
    /// ファイルパスからMediaInfoレコードを生成する
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static MediaInfo ToMediaInfo(string filePath)
    {
        // 拡張子を取得
        var extension = Path.GetExtension(filePath);

        // 適用可能な拡張子か確認
        if (false == AvailableExtensions.Contains(extension))
        {
            throw new ArgumentException("この拡張子は対応していません", filePath);
        }

        // FFProbeを使用してメディアファイル情報を取得する
        var analysis = FFProbe.Analyse(filePath);
        var video = analysis.PrimaryVideoStream ?? throw new ArgumentException("このファイルは動画ファイルではない可能性があります。", filePath);

        // MediaInfoオブジェクト生成
        return new MediaInfo
        {
            FilePath = filePath,
            FileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath),
            Extension = Path.GetExtension(filePath),
            Width = video.Width,
            Height = video.Height,
            Duration = video.Duration,
            FrameRate = video.FrameRate,
            VideoCodec = video.CodecName,
            FileSize = new FileInfo(filePath).Length
        };
    }

    /// <summary>
    /// ファイルパスからMediaInfoレコードを生成する（非同期）
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static async Task<MediaInfo> ToMediaInfoAsync(string filePath)
    {
        // 拡張子を取得
        var extension = Path.GetExtension(filePath);

        // 適用可能な拡張子か確認
        if (false == AvailableExtensions.Contains(extension))
        {
            throw new ArgumentException("この拡張子は対応していません", filePath);
        }

        // FFProbeを使用してメディアファイル情報を取得する
        var analysis = await FFProbe.AnalyseAsync(filePath);
        var video = analysis.PrimaryVideoStream ?? throw new ArgumentException("このファイルは動画ファイルではない可能性があります。", filePath);

        // MediaInfoオブジェクト生成
        return new MediaInfo
        {
            FilePath = filePath,
            FileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath),
            Extension = Path.GetExtension(filePath),
            Width = video.Width,
            Height = video.Height,
            Duration = video.Duration,
            FrameRate = video.FrameRate,
            VideoCodec = video.CodecName,
            FileSize = new FileInfo(filePath).Length
        };
    }
}
