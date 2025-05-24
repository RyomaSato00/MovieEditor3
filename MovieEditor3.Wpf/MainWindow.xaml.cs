using System.Reactive;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using MaterialDesignThemes.Wpf;

using MovieEditor3.Wpf.Programs;
using MovieEditor3.Wpf.ViewModels;

namespace MovieEditor3.Wpf;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    /// メインウィンドウのビューモデル
    /// </summary>
    private readonly MainWindowViewModel _mainWindowViewModel;

    public MainWindow()
    {
        InitializeComponent();

        // ViewModel初期化
        _mainWindowViewModel = new MainWindowViewModel(JsonHandler.Workspace);

        // WindowとViewModelの紐づけ
        DataContext = _mainWindowViewModel;

        Closing += MainWindow_Closing;

    }

    /// <summary>
    /// ウィンドウが閉じられる際の処理
    /// </summary>
    /// <param name="sender">イベント送信元</param>
    /// <param name="e">イベント引数</param>
    private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        _mainWindowViewModel.SaveSetting(JsonHandler.Workspace);
    }

    /// <summary>
    /// コマンド確認ダイアログを表示します
    /// </summary>
    public void ShowCommandCheckDialog()
    {
        var dialog = FindResource("CommandCheckDialog");
        DialogHost.Show(dialog, nameof(MainWindow));
    }

    /// <summary>
    /// 進捗ダイアログを表示します
    /// </summary>
    public void ShowProgressDialog()
    {
        var dialog = FindResource("ProgressDialog");
        DialogHost.Show(dialog, nameof(MainWindow));
    }

    /// <summary>
    /// 削除確認ダイアログを表示します
    /// </summary>
    public void ShowDeleteDialog()
    {
        var dialog = FindResource("DeleteDialog");
        DialogHost.Show(dialog, nameof(MainWindow));
    }

    /// <summary>
    /// 設定ダイアログを表示します
    /// </summary>
    public void ShowSettingDialog()
    {
        var dialog = FindResource("SettingDialog");
        DialogHost.Show(dialog, nameof(MainWindow));
    }
}
