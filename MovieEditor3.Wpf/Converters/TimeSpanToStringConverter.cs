
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MovieEditor3.Wpf.Converters;

/// <summary>
/// TimeSpanとstring（TextBox)間の変換を行うコンバータ
/// </summary>
internal class TimeSpanToStringConverter : IValueConverter
{

    /// <summary>
    /// TimeSpan型から文字列への変換を行います
    /// </summary>
    /// <param name="value">変換元のTimeSpan値</param>
    /// <param name="targetType">変換先の型</param>
    /// <param name="parameter">コンバーターパラメーター</param>
    /// <param name="culture">カルチャー情報</param>
    /// <returns>フォーマットされた時間文字列、または変換できない場合はUnsetValue</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is TimeSpan point)
        {
            return point.ToString("mm\\:ss\\.fff");
        }
        else
        {
            return DependencyProperty.UnsetValue;
        }
    }

    /// <summary>
    /// 文字列からTimeSpan型への変換を行います
    /// </summary>
    /// <param name="value">変換元の文字列</param>
    /// <param name="targetType">変換先の型</param>
    /// <param name="parameter">コンバーターパラメーター</param>
    /// <param name="culture">カルチャー情報</param>
    /// <returns>変換されたTimeSpan値、または変換に失敗した場合はnull</returns>
    public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var text = (string)value;

        try
        {
            var parts = text.Split(':');
            var seconds = 0.0;

            // 後ろ（秒）から足していく。分は60×valueを足し合わせる。時は60^2×value…
            for (var i = 0; i < parts.Length; i++)
            {
                seconds += Math.Pow(60, i) * double.Parse(parts[parts.Length - 1 - i]);
            }

            return TimeSpan.FromSeconds(seconds);
        }
        catch (Exception e)
        {
            System.Diagnostics.Debug.WriteLine($"{e}");
            return null;
        }
    }
}
