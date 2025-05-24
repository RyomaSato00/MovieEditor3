using System.IO;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using MaterialDesignThemes.Wpf;

using Microsoft.Win32;

using MovieEditor3.Wpf.Programs;

namespace MovieEditor3.Wpf.ViewModels;

/// <summary>
/// 設定ダイアログのビューモデルクラス
/// </summary>
internal partial class SettingDialogViewModel : ObservableObject
{
    /// <summary>
    /// ユーザー設定のバックアップ
    /// </summary>
    public UserSetting UserSettingBackup { get; } = new();

    /// <summary>
    /// 出力先ディレクトリのパス
    /// </summary>
    [ObservableProperty] private string _outputDirectory;

    /// <summary>
    /// ダイアログをキャンセルして閉じます
    /// </summary>
    /// <remarks>
    /// 設定の変更は保存されません
    /// </remarks>
    [RelayCommand]
    private void Cancel()
    {
        DialogHost.Close(nameof(MainWindow));
        ReadSetting();
    }

    /// <summary>
    /// 設定を保存してダイアログを閉じます
    /// </summary>
    [RelayCommand]
    private void Decide()
    {
        DialogHost.Close(nameof(MainWindow));
        WriteSetting();
    }

    /// <summary>
    /// 出力先ディレクトリを選択するダイアログを表示します
    /// </summary>
    [RelayCommand]
    private void SelectOutputDirectory()
    {
        var result = GetDrectoryPathFromExplorer(OutputDirectory);
        if (result is string path)
        {
            OutputDirectory = path;
        }
    }

    public SettingDialogViewModel(UserSetting userSetting)
    {
        UserSetting.Copy(userSetting, UserSettingBackup);
        OutputDirectory = userSetting.OutputDirectory;
    }

    /// <summary>
    /// バックアップから設定を読み込みます
    /// </summary>
    private void ReadSetting()
    {
        OutputDirectory = UserSettingBackup.OutputDirectory;
    }

    /// <summary>
    /// 現在の設定をバックアップに書き込みます
    /// </summary>
    private void WriteSetting()
    {
        UserSettingBackup.OutputDirectory = OutputDirectory;
    }

    /// <summary>
    /// エクスプローラーからディレクトリパスを取得します
    /// </summary>
    /// <param name="initialPath">初期ディレクトリパス</param>
    /// <returns>選択されたディレクトリパス、またはキャンセルされた場合はnull</returns>
    private static string? GetDrectoryPathFromExplorer(string initialPath)
    {
        string? result;
        var folderDialog = new OpenFolderDialog
        {
            Title = "フォルダ指定",
            Multiselect = false,
        };

        try
        {
            var fullPath = Path.GetFullPath(initialPath);

            if (Directory.Exists(fullPath))
            {
                folderDialog.InitialDirectory = fullPath;
            }

            result = true == folderDialog.ShowDialog() ? folderDialog.FolderName : null;
        }
        catch (Exception)
        {
            result = true == folderDialog.ShowDialog() ? folderDialog.FolderName : null;
        }

        return result;
    }
}
