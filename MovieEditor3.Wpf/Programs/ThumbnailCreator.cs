using System.Diagnostics;
using System.IO;

namespace MovieEditor3.Wpf.Programs;

/// <summary>
/// メディアファイルからサムネイル画像を生成するユーティリティクラス
/// </summary>
/// <summary>
/// Provides utility methods for creating thumbnails from media files using FFmpeg.
/// </summary>
/// <remarks>
/// This class handles thumbnail generation for media files, supporting both synchronous and asynchronous operations.
/// It can create thumbnails at a specific time point or from the last frame of a media file.
/// </remarks>
internal class ThumbnailCreator
{
    /// <summary>
    /// FFMPEGの実行ファイルパス
    /// </summary>
    private const string FFMPEG_EXE = "ffmpeg";

    /// <summary>
    /// サムネイル画像の拡張子
    /// </summary>
    private const string IMAGE_EXTENSION = ".jpg";

    /// <summary>
    /// サムネイル画像を生成する
    /// </summary>
    /// <param name="mediaPath">メディアファイルのパス</param>
    /// <param name="point">サムネイル画像を切り出す時間位置</param>
    /// <returns>生成されたサムネイル画像のファイルパス</returns>
    public static string CreateThumbnail(string mediaPath, TimeSpan point)
    {
        // サムネイル画像ファイルを置いておくディレクトリが存在？
        if (false == Directory.Exists(App.ThumbnailDirectory))
        {
            Directory.CreateDirectory(App.ThumbnailDirectory);
        }

        // サムネイル画像ファイルパス生成
        var thumbnailPath = ToThumbnailPath(mediaPath, point);

        // ファイルパスの重複を回避
        thumbnailPath = Utilities.RenameOnlyPath(thumbnailPath);

        // ffmpeg用コマンドライン引数を生成
        var arguments = ConfigureArguments(mediaPath, thumbnailPath, point);

        // サムネイル生成
        CreateImageFile(arguments);

        return thumbnailPath;
    }

    /// <summary>
    /// サムネイル画像を非同期で生成する
    /// </summary>
    /// <param name="mediaPath">メディアファイルのパス</param>
    /// <param name="point">サムネイル画像を切り出す時間位置</param>
    /// <returns>生成されたサムネイル画像のファイルパス</returns>
    public static async Task<string> CreateThumbnailAsync(string mediaPath, TimeSpan point)
    {
        // サムネイル画像ファイルを置いておくディレクトリが存在？
        if (false == Directory.Exists(App.ThumbnailDirectory))
        {
            Directory.CreateDirectory(App.ThumbnailDirectory);
        }

        // サムネイル画像ファイルパス生成
        var thumbnailPath = ToThumbnailPath(mediaPath, point);

        // ファイルパスの重複を回避
        thumbnailPath = Utilities.RenameOnlyPath(thumbnailPath);

        // ffmpeg用コマンドライン引数を生成
        var arguments = ConfigureArguments(mediaPath, thumbnailPath, point);

        // サムネイル生成
        await CreateImageFileAsync(arguments);

        return thumbnailPath;
    }

    /// <summary>
    /// メディアファイルの最後のサムネイル画像を生成する
    /// </summary>
    /// <param name="mediaPath">サムネイル画像を生成するメディアファイルのパス</param>
    /// <returns>生成されたサムネイル画像のファイルパス</returns>
    public static string CreateLastThumbnail(string mediaPath)
    {
        // サムネイル画像ファイルを置いておくディレクトリが存在？
        if (false == Directory.Exists(App.ThumbnailDirectory))
        {
            Directory.CreateDirectory(App.ThumbnailDirectory);
        }

        // サムネイル画像ファイルパス生成
        var thumbnailPath = ToLastThumbnailPath(mediaPath);

        // ファイルパスの重複を回避
        thumbnailPath = Utilities.RenameOnlyPath(thumbnailPath);

        // ffmpeg用コマンドライン引数を生成
        var arguments = ConfigureLastThumbnailArguments(mediaPath, thumbnailPath);

        // サムネイル生成
        CreateImageFile(arguments);

        return thumbnailPath;
    }

    /// <summary>
    /// メディアファイルの最後のサムネイル画像を非同期で生成する
    /// </summary>
    /// <param name="mediaPath">サムネイル画像を生成するメディアファイルのパス</param>
    /// <returns>生成されたサムネイル画像のファイルパス</returns>
    public static async Task<string> CreateLastThumbnailAsync(string mediaPath)
    {
        // サムネイル画像ファイルを置いておくディレクトリが存在？
        if (false == Directory.Exists(App.ThumbnailDirectory))
        {
            Directory.CreateDirectory(App.ThumbnailDirectory);
        }

        // サムネイル画像ファイルパス生成
        var thumbnailPath = ToLastThumbnailPath(mediaPath);

        // ファイルパスの重複を回避
        thumbnailPath = Utilities.RenameOnlyPath(thumbnailPath);

        // ffmpeg用コマンドライン引数を生成
        var arguments = ConfigureLastThumbnailArguments(mediaPath, thumbnailPath);

        // サムネイル生成
        await CreateImageFileAsync(arguments);

        return thumbnailPath;
    }

    /// <summary>
    /// メディアファイル名と時間からサムネイル画像のファイルパスを生成
    /// </summary>
    /// <param name="mediaPath">メディアファイルのパス</param>
    /// <param name="point">サムネイル画像を切り出す時間位置</param>
    /// <returns>生成されたサムネイル画像のファイルパス</returns>
    private static string ToThumbnailPath(string mediaPath, TimeSpan point)
    {
        var originalName = Path.GetFileNameWithoutExtension(mediaPath);
        var fileName = $"{originalName}_{point:hhmmssfff}{IMAGE_EXTENSION}";
        return Path.Combine(App.ThumbnailDirectory, fileName);
    }

    /// <summary>
    /// メディアファイル名と時間からサムネイル画像のファイルパスを生成（最終フレーム用）
    /// </summary>
    /// <param name="mediaPath">メディアファイルのパス</param>
    /// <param name="point">サムネイル画像を切り出す時間位置</param>
    /// <returns>生成されたサムネイル画像のファイルパス</returns>
    private static string ToLastThumbnailPath(string mediaPath)
    {
        var originalName = Path.GetFileNameWithoutExtension(mediaPath);
        var fileName = $"{originalName}_last{IMAGE_EXTENSION}";
        return Path.Combine(App.ThumbnailDirectory, fileName);
    }

    /// <summary>
    /// ffmpegを使用してサムネイル画像を生成
    /// </summary>
    /// <param name="arguments">処理コマンド</param>
    private static void CreateImageFile(string arguments)
    {
        // 処理条件設定
        var processInfo = new ProcessStartInfo(FFMPEG_EXE)
        {
            Arguments = arguments,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process { StartInfo = processInfo };

        // 処理開始
        process.Start();

        // 処理待機
        process.WaitForExit();
    }

    /// <summary>
    /// ffmpegを使用してサムネイル画像を生成（非同期）
    /// </summary>
    /// <param name="arguments">処理コマンド</param>
    private static async Task CreateImageFileAsync(string arguments)
    {
        // 処理条件設定
        var processInfo = new ProcessStartInfo(FFMPEG_EXE)
        {
            Arguments = arguments,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process { StartInfo = processInfo };

        // 処理開始
        process.Start();

        // 処理待機（非同期）
        await process.WaitForExitAsync();
    }

    /// <summary>
    /// ffmpegに渡すコマンドライン引数を組み立てる
    /// </summary>
    /// <param name="mediaPath">メディアファイルのパス</param>
    /// <param name="imagePath">サムネイル画像のパス</param>
    /// <param name="point">サムネイル画像を切り出す時間位置</param>
    /// <returns>ffmpegに渡すコマンドライン引数</returns>
    private static string ConfigureArguments(string mediaPath, string imagePath, TimeSpan point)
    {
        return $"-y -i \"{mediaPath}\" -ss {point:hh\\:mm\\:ss\\.fff} -vframes 1 -q:v 2 \"{imagePath}\"";
    }

    /// <summary>
    /// ffmpegに渡すコマンドライン引数を組み立てる
    /// </summary>
    /// <param name="mediaPath">メディアファイルのパス</param>
    /// <param name="imagePath">サムネイル画像のパス</param>
    /// <returns>ffmpegに渡すコマンドライン引数</returns>
    private static string ConfigureLastThumbnailArguments(string mediaPath, string imagePath)
    {
        return $"-y -sseof -1 -i \"{mediaPath}\" -vframes 1 -q:v 2 \"{imagePath}\"";
    }
}
