using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using MaterialDesignThemes.Wpf;

namespace MovieEditor3.Wpf.ViewModels;

/// <summary>
/// はい/いいえの選択ダイアログのビューモデルクラス
/// </summary>
internal partial class YesNoDialogViewModel : ObservableObject
{
    /// <summary>
    /// ダイアログに表示するメッセージ
    /// </summary>
    [ObservableProperty] private string _message = string.Empty;

    /// <summary>
    /// ダイアログの回答を処理します
    /// </summary>
    /// <param name="isYes">「はい」が選択された場合はtrue、「いいえ」が選択された場合はfalse</param>
    [RelayCommand]
    private void Answer(bool isYes)
    {
        DialogHost.Close(null);
        _answered?.SetResult(isYes);
    }

    /// <summary>
    /// 回答結果を通知するためのタスク完了ソース
    /// </summary
    private TaskCompletionSource<bool>? _answered;

    /// <summary>
    /// ユーザーの回答を待機します
    /// </summary>
    /// <returns>「はい」が選択された場合はtrue、「いいえ」が選択された場合はfalse</returns>
    public async Task<bool> WaitForAnswer()
    {
        _answered = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var isYes = await _answered.Task;
        _answered = null;
        return isYes;
    }
}
