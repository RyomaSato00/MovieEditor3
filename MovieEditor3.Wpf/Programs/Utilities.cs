using System.Diagnostics;
using System.IO;

namespace MovieEditor3.Wpf.Programs;

internal static class Utilities
{
    /// <summary>
    /// ファイル名が重複している場合、重複しないファイル名に変更
    /// </summary>
    /// <param name="originalPath">ファイルパス</param>
    /// <returns>重複しないファイルパス</returns>
    public static string RenameOnlyPath(string originalPath)
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
    /// エクスプローラーの実行ファイル名
    /// </summary>
    private const string EXPLORER_EXE = "EXPLORER.EXE";

    /// <summary>
    /// 指定されたディレクトリパスをWindowsエクスプローラーで開きます
    /// </summary>
    /// <param name="dirPath">開くディレクトリのパス</param>
    /// <returns>エクスプローラーの起動に成功した場合はtrue、失敗した場合はfalse</returns>
    public static async Task<bool> OpenExplorerAsync(string dirPath)
    {
        var isSuccess = false;

        if (Directory.Exists(dirPath))
        {
            var info = new ProcessStartInfo(EXPLORER_EXE)
            {
                Arguments = $"\"{dirPath}\"",
                UseShellExecute = true,
            };

            using var process = new Process { StartInfo = info };
            isSuccess = process.Start();
            await process.WaitForExitAsync();
        }

        return isSuccess;
    }
}
