using System.Reactive;
using System.Reactive.Subjects;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using MaterialDesignThemes.Wpf;

namespace MovieEditor3.Wpf.ViewModels;

/// <summary>
/// コマンド確認ダイアログのビューモデルクラス
/// </summary>
internal partial class CommandCheckDialogViewModel : ObservableObject
{
    /// <summary>
    /// コマンド確認情報の配列
    /// </summary>
    [ObservableProperty] private CommandCheckInfo[]? _commandProperties = null;

    /// <summary>
    /// チェック結果を処理し、ダイアログを閉じます
    /// </summary>
    /// <param name="isChecked">チェック結果</param>
    [RelayCommand]
    private void Checked(bool isChecked)
    {
        DialogHost.Close(null);
        _commandChecked?.SetResult(isChecked);
    }

    /// <summary>
    /// コマンドチェック結果を通知するためのタスク完了ソース
    /// </summary>
    private TaskCompletionSource<bool>? _commandChecked;

    /// <summary>
    /// コマンドチェックの完了を待機します
    /// </summary>
    /// <returns>チェック結果</returns>
    public async Task<bool> WaitForCommandChecked()
    {
        _commandChecked = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var isChecked = await _commandChecked.Task;
        _commandChecked = null;
        return isChecked;
    }
}

/// <summary>
/// コマンド確認情報を保持するクラス
/// </summary>
internal partial class CommandCheckInfo(string command, string thumbnailPath) : ObservableObject
{
    /// <summary>
    /// コマンド文字列
    /// </summary>
    [ObservableProperty] private string _command = command;

    /// <summary>
    /// サムネイル画像のパス
    /// </summary>
    public string ThumbnailPath { get; } = thumbnailPath;
}
