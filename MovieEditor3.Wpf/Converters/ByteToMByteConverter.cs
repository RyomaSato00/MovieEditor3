using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MovieEditor3.Wpf.Converters;

[ValueConversion(typeof(long), typeof(float))]
public class ByteToMByteConverter : IValueConverter
{
    /// <summary>
    /// long型のファイルサイズ値をfloat型のMbyte単位に変換する
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var result = DependencyProperty.UnsetValue;

        if (value is long byteSize)
        {
            // B単位からkB単位に変換
            byteSize >>= 10;

            // 実数に変換にし、kB単位からMB単位に変換
            result = (float)byteSize / 1000;
        }

        return result;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
