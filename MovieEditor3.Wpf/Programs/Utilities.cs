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
}
