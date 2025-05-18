using System.Diagnostics;
using System.Reactive;

namespace MovieEditor3.Wpf.Programs;

/// <summary>
/// コマンドプロセスを並列実行するためのユーティリティクラス
/// </summary>
internal class ParallelCommandProcessor
{
    /// <summary>
    /// 指定されたコマンドプロセス情報のコレクションを並列実行します
    /// </summary>
    /// <param name="items">実行するコマンドプロセス情報のコレクション</param>
    public static void Run(IEnumerable<CommandProcessInfo> items)
    {
        items.AsParallel()
        .WithDegreeOfParallelism(Environment.ProcessorCount)
        .ForAll(static item =>
        {
            try
            {
                item.CommandProcess.Start();
                item.CommandProcess.WaitForExit();
                item.Completed();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        });
    }

    /// <summary>
    /// 指定されたコマンドプロセス情報のコレクションを強制終了します
    /// </summary>
    /// <param name="items">強制終了するコマンドプロセス情報のコレクション</param>
    public static void Kill(IEnumerable<CommandProcessInfo> items)
    {
        foreach (var item in items)
        {
            item.CommandProcess.Kill();
        }
    }
}

/// <summary>
/// コマンドプロセスの実行情報を管理するクラス
/// </summary>
internal class CommandProcessInfo
{
    /// <summary>
    /// 実行するプロセスのファイル名
    /// </summary>
    private const string PROCESS_FILE_NAME = "ffmpeg";

    /// <summary>
    /// コマンド文字列を取得します
    /// </summary>
    public string Command { get; }

    /// <summary>
    /// コマンドプロセスを取得します
    /// </summary>
    public Process CommandProcess { get; }

    /// <summary>
    /// プロセスが完了したかどうかを示す値を取得します
    /// </summary>
    public bool IsCompleted { get; private set; } = false;

    public CommandProcessInfo(string command)
    {
        Command = command;

        var info = new ProcessStartInfo
        {
            FileName = PROCESS_FILE_NAME,
            Arguments = command,
            UseShellExecute = false,
            CreateNoWindow = true,
        };
        CommandProcess = new Process { StartInfo = info };
    }

    /// <summary>
    /// プロセスの完了を確定します
    /// </summary>
    public void Completed()
    {
        IsCompleted = true;
    }
}
