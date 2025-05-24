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

namespace MovieEditor3.Wpf.Views;

/// <summary>
/// Interaction logic for MediaListView.xaml
/// </summary>
public partial class MediaListView : UserControl
{
    public MediaListView()
    {
        InitializeComponent();
    }

    public void ShowWaitingDialog()
    {
        var dialog = FindResource("WaitingDialog");
        DialogHost.Show(dialog, nameof(MediaListView));
    }
}

