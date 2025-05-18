using System.Windows.Threading;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using MaterialDesignThemes.Wpf;

using MovieEditor3.Wpf.Programs;

namespace MovieEditor3.Wpf.ViewModels;

/// <summary>
/// 処理進捗状況を表示するダイアログのビューモデルクラス
/// </summary>
internal partial class ProgressDialogViewModel : ObservableObject
{
    /// <summary>
    /// 完了した処理の数
    /// </summary>
    [ObservableProperty] private int _progressCount;

    /// <summary>
    /// 全体の処理数
    /// </summary>
    [ObservableProperty] private int _processCount;

    /// <summary>
    /// 処理をキャンセルします
    /// </summary>
    [RelayCommand]
    private void Cancel()
    {
        if (_processItems is not null)
        {
            ParallelCommandProcessor.Kill(_processItems);
        }
    }

    /// <summary>
    /// UI更新の間隔（ミリ秒）
    /// </summary>
    private const int UI_INTERVAL_MSEC = 100;

    /// <summary>
    /// UI更新用タイマー
    /// </summary>
    private readonly DispatcherTimer _uiUpdateTimer;

    /// <summary>
    /// 処理中のプロセス情報配列
    /// </summary
    private CommandProcessInfo[]? _processItems;

    public ProgressDialogViewModel()
    {
        _uiUpdateTimer = new DispatcherTimer(
            TimeSpan.FromMilliseconds(UI_INTERVAL_MSEC),
            DispatcherPriority.Normal,
            Tick,
            Dispatcher.CurrentDispatcher
        );
        _uiUpdateTimer.Stop();
    }

    /// <summary>
    /// ダイアログを閉じます
    /// </summary>
    public void Close()
    {
        _uiUpdateTimer.Stop();
        DialogHost.Close(null);
        _processItems = null;
    }

    /// <summary>
    /// 進捗状況を設定し、進捗監視を開始します
    /// </summary>
    /// <param name="processItems">処理中のプロセス情報配列</param>
    public void SetProgress(CommandProcessInfo[] processItems)
    {
        _processItems = processItems;
        ProcessCount = processItems.Length;
        _uiUpdateTimer.Start();
    }

    /// <summary>
    /// 定期的に進捗状況を更新します
    /// </summary>
    /// <param name="sencer">イベント送信元</param>
    /// <param name="e">イベント引数</param>
    private void Tick(object? sencer, EventArgs e)
    {
        if (_processItems is null) return;

        var progressCount = 0;
        foreach (var item in _processItems)
        {
            if (item.IsCompleted)
            {
                progressCount++;
            }
        }

        ProgressCount = progressCount;
    }
}
