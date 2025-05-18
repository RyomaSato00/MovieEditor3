using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls.Primitives;

using MovieEditor3.Wpf.Programs;

namespace MovieEditor3.Wpf;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    /// <summary> キャッシュ用ディレクトリ </summary>
    public static readonly string CacheDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Cache");

    /// <summary> サムネイル画像を置いておくディレクトリ </summary>
    public static readonly string ThumbnailDirectory = Path.Combine(CacheDirectory, "thumbnails");

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // キャッシュフォルダ作成
        if (false == Directory.Exists(CacheDirectory))
        {
            Directory.CreateDirectory(CacheDirectory);
        }
        else
        {
            DeleteCacheFiles(ThumbnailDirectory);
        }

        // Cache//report.txtにデバッグ用テキストを出力
        var reportFile = Path.Combine(CacheDirectory, "report.txt");
        Trace.Listeners.Add(new TextWriterTraceListener(reportFile));
        Debug.AutoFlush = true;

        Debug.WriteLine($"[{DateTime.Now:yyyy/MM/dd HH:mm:ss:fff}] The application has started.");

        // JSONファイルからユーザー設定を読み込む
        JsonHandler.Load();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        // JSONファイルにユーザー設定を保存
        JsonHandler.Save();

        base.OnExit(e);

        Debug.WriteLine($"[{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff}] The application has exited.");

        foreach (TraceListener listener in Trace.Listeners)
        {
            listener.Flush();
            listener.Close();
        }
    }

    private static void DeleteCacheFiles(string directory)
    {
        // キャッシュディレクトリが存在しないなら、何もしない
        if (false == Directory.Exists(directory)) return;

        // キャッシュディレクトリ内のすべてのファイルパスを取得
        var files = Directory.GetFiles(directory);

        // キャッシュファイルを削除
        foreach (var file in files)
        {
            try
            {
                File.Delete(file);
            }
            catch (FileNotFoundException)
            {
                continue;
            }
            catch (UnauthorizedAccessException)
            {
                continue;
            }
        }

    }
}

