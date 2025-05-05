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
        _mainWindowViewModel = new MainWindowViewModel();

        // WindowとViewModelの紐づけ
        DataContext = _mainWindowViewModel;
    }
}
