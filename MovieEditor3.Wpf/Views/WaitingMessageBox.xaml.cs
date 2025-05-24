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

namespace MovieEditor3.Wpf.Views;

/// <summary>
/// Interaction logic for WaitingDialog.xaml
/// </summary>
public partial class WaitingDialog : UserControl
{
    public static readonly DependencyProperty TextProperty =
    DependencyProperty.Register(nameof(Text), typeof(string), typeof(WaitingDialog), new PropertyMetadata(string.Empty));

    public WaitingDialog()
    {
        InitializeComponent();
    }

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }
}

