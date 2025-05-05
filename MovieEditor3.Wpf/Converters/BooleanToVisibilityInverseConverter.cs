using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MovieEditor3.Wpf.Converters;

public class BooleanToVisibilityInverseConverter : IValueConverter
{
    /// <summary>
    /// true → Visible, false → Collapsed
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var result = DependencyProperty.UnsetValue;

        if (value is bool isVisible)
        {
            if (isVisible)
            {
                result = Visibility.Collapsed;
            }
            else
            {
                result = Visibility.Visible;
            }
        }

        return result;
    }

    /// <summary>
    /// Visible → false, Collapsed → true, Hidden → true
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var result = DependencyProperty.UnsetValue;

        if (value is Visibility visibility)
        {
            switch(visibility)
            {
            case Visibility.Visible:
                result = false;
                break;

            case Visibility.Collapsed:
                result = true;
                break;

            case Visibility.Hidden:
                result = true;
                break;

            default:
                break;
            }
        }

        return result;
    }
}
