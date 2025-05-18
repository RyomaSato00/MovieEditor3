using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MovieEditor3.Wpf.Converters;

public class BooleanToVisibilityCustomConverter : IValueConverter
{
    /// <summary>
    /// true → Visible, false → Hidden
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
                result = Visibility.Visible;
            }
            else
            {
                result = Visibility.Hidden;
            }
        }

        return result;
    }

    /// <summary>
    /// Visible → true, Collapsed → false, Hidden → false
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
                result = true;
                break;

            case Visibility.Collapsed:
                result = false;
                break;

            case Visibility.Hidden:
                result = false;
                break;

            default:
                break;
            }
        }

        return result;
    }
}
