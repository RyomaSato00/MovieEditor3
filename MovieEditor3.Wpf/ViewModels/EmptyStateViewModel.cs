using System.IO;
using System.Reactive.Subjects;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Microsoft.Win32;

using MovieEditor3.Wpf.Programs;

namespace MovieEditor3.Wpf.ViewModels;

internal partial class EmptyStateViewModel : ObservableObject
{
    /// <summary>
    /// メディアファイルが開かれたときに通知されるObservable
    /// </summary>
    public IObservable<string[]> MediaFilesOpened => _mediaFilesOpened;

    /// <summary>
    /// メディアファイルが開かれたときに発行されるSubject
    /// </summary>
    private readonly Subject<string[]> _mediaFilesOpened = new();

    /// <summary>
    /// 最近使用したメディアのディレクトリパス
    /// </summary>
    private string _recentryUsedDirPath = string.Empty;

    public EmptyStateViewModel(UserSetting userSetting)
    {
        _recentryUsedDirPath = userSetting.RecentryUsedDirPath;
    }

    /// <summary>
    /// 設定値の保存
    /// </summary>
    /// <param name="userSetting">ユーザー設定</param>
    public void SaveSetting(UserSetting userSetting)
    {
        userSetting.RecentryUsedDirPath = _recentryUsedDirPath;
    }

    /// <summary>
    /// フォルダを開くダイアログを表示し、選択されたファイルのパスを通知します。
    /// また、最後に選択されたファイルのディレクトリパスを保存します。
    /// </summary>
    [RelayCommand]
    private void OpenFolder()
    {
        string[]? result;
        var fileDialog = new OpenFileDialog()
        {
            Title = "メディアファイルを選択",
            Multiselect = true,
        };

        try
        {
            var fullPath = Path.GetFullPath(_recentryUsedDirPath);

            if (Directory.Exists(fullPath))
            {
                fileDialog.InitialDirectory = fullPath;
            }

            result = true == fileDialog.ShowDialog() ? fileDialog.FileNames : null;
        }
        catch (Exception)
        {
            result = true == fileDialog.ShowDialog() ? fileDialog.FileNames : null;
        }

        _mediaFilesOpened.OnNext(result ?? Array.Empty<string>());

        // 最後に選択されたファイルのディレクトリパスを保存
        if (result is not null && result.Length > 0)
        {
            _recentryUsedDirPath = Path.GetDirectoryName(result[^1]) ?? string.Empty;
        }
    }
}
