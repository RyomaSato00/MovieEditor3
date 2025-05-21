using System.IO;
using System.Windows;

namespace MovieEditor3.Wpf.Programs;

internal class FFmpegCommandConverter
{
    private const string MOVIE_FORMAT = ".mp4";

    private const string IMAGE_FORMAT = ".png";

    private static readonly Dictionary<RotateID, string> RotateCommands = new()
    {
        { RotateID.Rotate90, "transpose=1" },
        { RotateID.Rotate180, "transpose=1,transpose=1" },
        { RotateID.Rotate270, "transpose=2" },
    };

    /// <summary>
    /// 動画圧縮用コマンド生成処理
    /// </summary>
    /// <param name="itemInfo"></param>
    /// <param name="setting"></param>
    /// <param name="outputDirectory"></param>
    /// <param name="isOnly">出力ファイルの重複を回避する？</param>
    /// <returns></returns>
    public static string ToCompressCommand(ICompInfo info)
    {
        var argList = new List<string>
        {
            // 入力ファイルパス
            $"-y -i \"{info.FilePath}\""
        };

        // vfコマンドを取得
        var vf = ToVf(info);

        if (vf is not null)
        {
            argList.Add(vf);
        }

        // フレームレート
        if (0 < info.FrameRate)
        {
            argList.Add($"-r {info.FrameRate}");
        }

        // 動画コーデック
        if (string.IsNullOrWhiteSpace(info.Codec) == false)
        {
            argList.Add($"-c:v {info.Codec}");
        }

        // 音声削除
        if (info.IsAudioDisabled)
        {
            argList.Add("-an");
        }

        // トリミング開始位置
        if (info.StartPoint is not null)
        {
            argList.Add($"-ss {info.StartPoint:hh\\:mm\\:ss\\.fff}");
        }

        // トリミング終了位置
        if (info.EndPoint is not null)
        {
            argList.Add($"-to {info.EndPoint:hh\\:mm\\:ss\\.fff}");
        }

        // 出力先指定
        var output = Path.Combine(info.OutputDirectory, $"{info.OutputName}{MOVIE_FORMAT}");

        // 重複しないファイルパスに書き換える
        output = Utilities.RenameOnlyPath(output);

        argList.Add($"\"{output}\"");

        return string.Join(" ", argList);
    }

    /// <summary>
    /// 画像出力用コマンド生成処理
    /// </summary>
    /// <param name="itemInfo"></param>
    /// <param name="setting"></param>
    /// <param name="outputDirectory"></param>
    /// <returns></returns>
    public static string ToImagesCommand(IGenerateImagesInfo info)
    {
        var argList = new List<string>
        {
            // 入力ファイルパス
            $"-y -i \"{info.FilePath}\""
        };

        // 1秒間のフレーム数
        if (info.FramesPerSecond >= 1)
        {
            argList.Add($"-r {info.FramesPerSecond}");
        }

        // 総フレーム数
        if (info.CountOfFrames >= 1)
        {
            argList.Add($"-vframes {info.CountOfFrames}");
        }

        // 画像品質（数が大きいほど品質が低下し、容量が少なくなる）
        if (info.Quality >= 0)
        {
            argList.Add($"-q:v {info.Quality}");
        }

        // トリミング開始位置
        if (info.StartPoint is not null)
        {
            argList.Add($"-ss {info.StartPoint:hh\\:mm\\:ss\\.fff}");
        }

        // トリミング終了位置
        if (info.EndPoint is not null)
        {
            argList.Add($"-to {info.EndPoint:hh\\:mm\\:ss\\.fff}");
        }

        // 画像出力フォルダの作成
        Directory.CreateDirectory(Path.Combine(info.OutputDirectory, info.OutputName));

        // 出力先指定
        var output = Path.Combine(info.OutputDirectory, info.OutputName, $"{info.OutputName}_%06d.{IMAGE_FORMAT}");

        // 重複しないファイルパスに書き換える
        output = Utilities.RenameOnlyPath(output);

        argList.Add($"\"{output}\"");

        return string.Join(" ", argList);
    }

    /// <summary>
    /// VFコマンド生成処理
    /// </summary>
    /// <param name="itemInfo"></param>
    /// <param name="setting"></param>
    /// <param name="outputDirectory"></param>
    /// <returns></returns>
    private static string? ToVf(ICompInfo info)
    {
        // スケール指定あり？
        var isScaleReserved = info.ResizedWidth >= 0 || info.ResizedHeight >= 0;

        // クリッピング指定あり？
        var isClippingReserved = info.CropRect != Rect.Empty;

        // 回転指定あり？
        var isRotationReserved = info.Rotation != RotateID.None;

        // 速度倍率指定あり？
        var isSpeedReserved = info.PlaybackSpeed > 0;

        // すべて指定なしのとき
        if (isScaleReserved == false && isClippingReserved == false && isRotationReserved == false && isSpeedReserved == false)
        {
            return null;
        }
        else
        {
            var vfList = new List<string>();
            var vfSubList = new List<string>();
            double cropWidth = 0, cropHeight = 0;

            // クリッピング指定あり
            if (isClippingReserved)
            {
                var cropArg = ToCrop(info, out cropWidth, out cropHeight);

                if (cropArg is not null)
                {
                    vfList.Add(cropArg);
                }
            }

            // スケール指定あり
            if (isScaleReserved)
            {
                // 元動画のサイズを取得
                var originWidth = info.OriginalWidth;
                var originHeight = info.OriginalHeight;

                // クリッピング指定あり？
                if (isClippingReserved)
                {
                    // 元動画のサイズはクリッピング後のサイズとする
                    originWidth = (int)cropWidth;
                    originHeight = (int)cropHeight;
                }

                var scaleArg = ToScale(info.ResizedWidth, info.ResizedHeight, originWidth, originHeight);

                if (scaleArg is not null)
                {
                    vfList.Add(scaleArg);
                }
            }

            // 回転指定あり
            if (isRotationReserved)
            {
                var isRotateExists = RotateCommands.TryGetValue(info.Rotation, out var rotateCommand);

                if (isRotateExists && rotateCommand is not null)
                {
                    vfList.Add(rotateCommand);

                    // 回転後の角度を0度と定義する
                    vfSubList.Add("-metadata:s:v:0 rotate=0");
                }
            }

            // 速度倍率指定あり
            if (isSpeedReserved)
            {
                vfList.Add($"setpts=PTS/{info.PlaybackSpeed}");

                // 音声の速度も同時に変更
                vfSubList.Add($"-af atempo={info.PlaybackSpeed}");
            }

            return $"-vf \"{string.Join(',', vfList)}\" {string.Join(' ', vfSubList)}";
        }
    }

    /// <summary>
    /// スケールのコマンドを作成する
    /// </summary>
    /// <remarks>
    /// 値が0以下のときはAutoとする
    /// </remarks>
    /// <param name="info"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    private static string? ToScale(int width, int height, int originWidth, int originHeight)
    {
        // どちらも正値のときはそのまま
        if (0 < width && 0 < height)
        {
            return $"scale={width}:{height}";
        }
        // widthはAutoでheight指定のとき
        else if (0 >= width && 0 < height)
        {
            // 動画ファイルの解像度情報からWidthを自動計算
            int autoWidth = 0;

            // 0除算回避
            if (0 != originHeight)
            {
                autoWidth = height * originWidth / originHeight;
            }

            // 解像度は偶数にする必要がある。
            if (0 != autoWidth % 2)
            {
                autoWidth++;
            }

            return $"scale={autoWidth}:{height}";
        }
        // width指定でheightはAutoのとき
        else if (0 < width && 0 >= height)
        {
            // 動画ファイルの解像度情報からHeightを自動計算
            int autoHeight = 0;

            // 0除算回避
            if (0 != originWidth)
            {
                autoHeight = width * originHeight / originWidth;
            }

            // 解像度は偶数にする必要がある。
            if (0 != autoHeight % 2)
            {
                autoHeight++;
            }

            return $"scale={width}:{autoHeight}";
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// クリッピングのコマンドを作成する
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    private static string? ToCrop(ICompInfo info, out double clipWidth, out double clipHeight)
    {
        clipWidth = 0;
        clipHeight = 0;

        // クリッピング指定なしのときはnullを返す
        if (info.CropRect == Rect.Empty) return null;

        var x = info.CropRect.X >= 0 ? info.CropRect.X : 0;

        var y = info.CropRect.Y >= 0 ? info.CropRect.Y : 0;

        clipWidth = info.CropRect.Width + x <= info.OriginalWidth ? info.CropRect.Width : info.OriginalWidth - x;

        clipHeight = info.CropRect.Height + y <= info.OriginalHeight ? info.CropRect.Height : info.OriginalHeight - y;

        return $"crop={clipWidth:F2}:{clipHeight:F2}:{x:F2}:{y:F2}";
    }
}
