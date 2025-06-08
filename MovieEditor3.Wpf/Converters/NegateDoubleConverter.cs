using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MovieEditor3.Wpf.Converters;

/// <summary>
/// double型の値を反転（符号反転）する値コンバーター。
/// 例: 45.0 → -45.0
/// </summary>
public class NegateDoubleConverter : IValueConverter
{
    /// <summary>
    /// 値を反転して返します。
    /// </summary>
    /// <param name="value">バインディングされた値</param>
    /// <param name="targetType">ターゲットの型</param>
    /// <param name="parameter">追加パラメータ（未使用）</param>
    /// <param name="culture">カルチャ情報</param>
    /// <returns>反転した値、またはUnsetValue</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var result = DependencyProperty.UnsetValue;

        if (value is double doubleValue)
        {
            result = -doubleValue;
        }

        return result;
    }

    /// <summary>
    /// 値を反転して返します（双方向バインディング用）。
    /// </summary>
    /// <param name="value">バインディングされた値</param>
    /// <param name="targetType">ターゲットの型</param>
    /// <param name="parameter">追加パラメータ（未使用）</param>
    /// <param name="culture">カルチャ情報</param>
    /// <returns>反転した値、またはUnsetValue</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var result = DependencyProperty.UnsetValue;

        if (value is double doubleValue)
        {
            result = -doubleValue;
        }

        return result;
    }
}
