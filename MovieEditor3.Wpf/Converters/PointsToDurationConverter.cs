using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MovieEditor3.Wpf.Converters;

/// <summary>
/// 開始ポイントと終了ポイントから再生時間を計算し、表示用の文字列に変換するコンバーター
/// </summary>
public class PointsToDurationConverter : IMultiValueConverter
{
    /// <summary>
    /// 開始ポイントの配列インデックス
    /// </summary>
    private const int START_POINT_INDEX = 0;

    /// <summary>
    /// 終了ポイントの配列インデックス
    /// </summary>
    private const int END_POINT_INDEX = 1;

    /// <summary>
    /// デフォルト再生時間の配列インデックス
    /// </summary>
    private const int DEFAULT_DURATION = 2;

    /// <summary>
    /// 必要な配列の長さ
    /// </summary>
    private const int REQUIRED_LENGTH = 3;

    /// <summary>
    /// 再生時間の表示フォーマット
    /// </summary>
    private const string FORMAT_DURATION = "mm\\:ss\\.fff";

    /// <summary>
    /// 再生時間のヘッダーラベル
    /// </summary>
    private const string HEADER_LABEL = "再生時間：";

    /// <summary>
    /// 開始ポイントと終了ポイントから再生時間を計算し、表示用の文字列に変換します
    /// </summary>
    /// <param name="values">変換元の値の配列</param>
    /// <param name="targetType">変換先の型</param>
    /// <param name="parameter">コンバーターパラメーター</param>
    /// <param name="culture">カルチャー情報</param>
    /// <returns>再生時間を表す文字列</returns>
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        var duration = TimeSpan.Zero;

        if (REQUIRED_LENGTH <= values.Length)
        {
            var startPoint = values[START_POINT_INDEX] as TimeSpan?;
            var endPoint = values[END_POINT_INDEX] as TimeSpan?;
            var defaultDuration = values[DEFAULT_DURATION] as TimeSpan?;

            // 開始・終了ともに指定あり？
            if (startPoint is TimeSpan && endPoint is TimeSpan)
            {
                // 再生時間は終了ポイントから開始ポイントを引いたもの
                duration = endPoint.Value - startPoint.Value;
            }
            // 開始ポイントのみ指定あり？
            else if (startPoint is TimeSpan && defaultDuration is TimeSpan)
            {
                // 再生時間はデフォルトの再生時間から開始ポイントを引いたもの
                duration = defaultDuration.Value - startPoint.Value;
            }
            // 終了ポイントのみ指定あり？
            else if (endPoint is TimeSpan && defaultDuration is TimeSpan)
            {
                // 再生時間は0から終了ポイントまでの時間
                duration = endPoint.Value;
            }
            // 開始・終了ともに指定なし？
            else if (defaultDuration is TimeSpan)
            {
                // 再生時間はデフォルトの再生時間
                duration = defaultDuration.Value;
            }
        }

        var content = duration.ToString(FORMAT_DURATION);
        return string.Format("{0}{1}", HEADER_LABEL, content);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
