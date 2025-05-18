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
    private readonly MainWindowViewModel _mainWindowViewModel;

    public MainWindow()
    {
        InitializeComponent();

        // ViewModel初期化
        _mainWindowViewModel = new MainWindowViewModel(JsonHandler.Workspace);

        // WindowとViewModelの紐づけ
        DataContext = _mainWindowViewModel;

    }

    public void ShowCommandCheckDialog()
    {
        var dialog = FindResource("CommandCheckDialog");
        DialogHost.Show(dialog);
    }

    public void ShowProgressDialog()
    {
        var dialog = FindResource("ProgressDialog");
        DialogHost.Show(dialog);
    }

    public void ShowDeleteDialog()
    {
        var dialog = FindResource("DeleteDialog");
        DialogHost.Show(dialog);
    }
}
