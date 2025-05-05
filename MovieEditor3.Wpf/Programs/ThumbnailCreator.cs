using System.Diagnostics;
using System.IO;

namespace MovieEditor3.Wpf.Programs;

/// <summary>
/// メディアファイルからサムネイル画像を生成するユーティリティクラス
/// </summary>
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
        thumbnailPath = RenameOnlyPath(thumbnailPath);

        // サムネイル生成
        CreateImageFile(mediaPath, thumbnailPath, point);

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
        thumbnailPath = RenameOnlyPath(thumbnailPath);

        // サムネイル生成
        await CreateImageFileAsync(mediaPath, thumbnailPath, point);

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
    /// ファイル名が重複している場合、重複しないファイル名に変更
    /// </summary>
    /// <param name="originalPath">ファイルパス</param>
    /// <returns>重複しないファイルパス</returns>
    private static string RenameOnlyPath(string originalPath)
    {
        string result = originalPath;

        // ファイルパス重複あり？
        if (File.Exists(originalPath))
        {
            // ファイルが置かれたディレクトリを取得
            var parent = Path.GetDirectoryName(originalPath) ?? string.Empty;

            // 拡張子を除いたファイル名
            var pureName = Path.GetFileNameWithoutExtension(originalPath);

            // 拡張子
            var extension = Path.GetExtension(originalPath);

            // 複製カウンタ
            var duplicateCount = 1;

            do
            {
                // ファイルパス = ディレクトリ//ファイル名（カウンタ）.拡張子
                result = Path.Combine(parent, $"{pureName}({duplicateCount}){extension}");

                duplicateCount++;

            // 重複しないファイルパスが見つかるまでカウントアップ
            } while (File.Exists(result));

        }

        return result;
    }

    /// <summary>
    /// ffmpegを使用してサムネイル画像を生成
    /// </summary>
    /// <param name="mediaPath">メディアファイルのパス</param>
    /// <param name="imagePath">サムネイル画像のパス</param>
    /// <param name="point">サムネイル画像を切り出す時間位置</param>
    private static void CreateImageFile(string mediaPath, string imagePath, TimeSpan point)
    {
        // ffmpeg用コマンドライン引数を生成
        var arguments = ConfigureArguments(mediaPath, imagePath, point);

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
    /// <param name="mediaPath">メディアファイルのパス</param>
    /// <param name="imagePath">サムネイル画像のパス</param>
    /// <param name="point">サムネイル画像を切り出す時間位置</param>
    private static async Task CreateImageFileAsync(string mediaPath, string imagePath, TimeSpan point)
    {
        // ffmpeg用コマンドライン引数を生成
        var arguments = ConfigureArguments(mediaPath, imagePath, point);

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
}
