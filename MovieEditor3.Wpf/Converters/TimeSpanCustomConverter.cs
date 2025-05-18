using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MovieEditor3.Wpf.Converters;

public class TimeSpanCustomConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var result = DependencyProperty.UnsetValue;

        if (value is TimeSpan timeSpan)
        {
            result = timeSpan.ToString("mm\\:ss\\.fff");
        }

        return result;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
