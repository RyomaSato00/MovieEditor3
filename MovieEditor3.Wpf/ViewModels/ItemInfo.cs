using System.IO;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;

using MovieEditor3.Wpf.Programs;


namespace MovieEditor3.Wpf.ViewModels;

/// <summary>
/// メディアアイテムの情報を管理するクラス
/// </summary>
internal partial class ItemInfo : ObservableObject, IMovieEditViewProperty, IMediaPlayerProperty, ICompInfo, IGenerateImagesInfo
{
    /// <summary>
    /// デフォルトのパラメータ値
    /// </summary>
    private const int DEFAULT_PARAM_VALUE = -1;

    /// <summary>
    /// デフォルトのコーデック
    /// </summary>
    private const string DEFAULT_CODEC_HEVC = "hevc";

    /// <summary>
    /// デフォルトの1秒あたりの画像フレーム数
    /// </summary>
    private const int DEFAULT_FRAMES_PER_SECOND = 5;

    /// <summary>
    /// デフォルトの画像品質
    /// </summary>
    private const int DEFAULT_QUALITY = 0;

    public string FilePath { get; }

    public string FileName { get; }

    public string GuidName { get; }

    public MediaInfo OriginalMediaInfo { get; private set; } = MediaInfo.Empty;

    public int OriginalWidth => OriginalMediaInfo.Width;

    public int OriginalHeight => OriginalMediaInfo.Height;

    public TimeSpan DefaultDuration => OriginalMediaInfo.Duration;

    /// <summary>
    /// サムネイルのパス
    /// </summary>
    public string ThumbnailPath => StartPointImagePath;

    /// <summary>
    /// 選択状態
    /// </summary>
    [ObservableProperty] private bool _isSelected = false;

    [ObservableProperty] private TimeSpan? _startPoint = null;

    [ObservableProperty] private TimeSpan? _endPoint = null;

    [ObservableProperty] private string _startPointImagePath = string.Empty;

    [ObservableProperty] private string _endPointImagePath = string.Empty;

    [ObservableProperty] private Rect _cropRect = Rect.Empty;

    [ObservableProperty] private int _resizedWidth = DEFAULT_PARAM_VALUE;

    [ObservableProperty] private int _resizedHeight = DEFAULT_PARAM_VALUE;

    [ObservableProperty] private RotateID _rotation = RotateID.None;

    [ObservableProperty] private float _playbackSpeed = DEFAULT_PARAM_VALUE;

    [ObservableProperty] private float _frameRate = DEFAULT_PARAM_VALUE;

    [ObservableProperty] private string _codec = DEFAULT_CODEC_HEVC;

    [ObservableProperty] private bool _isAudioDisabled = true;

    [ObservableProperty] private int _framesPerSecond = DEFAULT_FRAMES_PER_SECOND;

    [ObservableProperty] private int _totalFrames = DEFAULT_PARAM_VALUE;

    [ObservableProperty] private int _quality = DEFAULT_QUALITY;

    /// <summary>
    /// アイテムの一意識別子
    /// </summary>
    private readonly Guid _identifier;

    /// <summary>
    /// 指定したファイルパスからアイテム情報を初期化します
    /// </summary>
    /// <param name="filePath">メディアファイルのパス</param>
    public ItemInfo(string filePath)
    {
        _identifier = Guid.NewGuid();
        FilePath = filePath;
        FileName = Path.GetFileNameWithoutExtension(filePath);
        GuidName = $"{FileName}_{_identifier}";
    }

    /// <summary>
    /// 既存のアイテム情報からコピーして新しいアイテム情報を初期化します
    /// </summary>
    /// <param name="itemInfo">コピー元のアイテム情報</param>
    public ItemInfo(ItemInfo itemInfo) : this(itemInfo.FilePath)
    {
        // メディア情報をコピー（ディープコピー）
        OriginalMediaInfo = itemInfo.OriginalMediaInfo with { };

        // プロパティをコピー
        Copy(itemInfo, this);
    }

    /// <summary>
    /// 指定したファイルパスからメディア情報とサムネイルを読み込む
    /// </summary>
    /// <param name="filePath">メディアファイルのパス</param>
    public void LoadInfo()
    {
        // メディア情報を読み込む
        OriginalMediaInfo = MediaInfo.ToMediaInfo(FilePath);

        StartPointImagePath = ThumbnailCreator.CreateThumbnail(FilePath, TimeSpan.Zero);
        EndPointImagePath = ThumbnailCreator.CreateLastThumbnail(FilePath);
    }

    partial void OnStartPointChanged(TimeSpan? value)
    {
        _ = SynchronizeStartPointImage(value);
    }

    partial void OnEndPointChanged(TimeSpan? value)
    {
        _ = SynchronizeEndPointImage(value);
    }

    /// <summary>
    /// 開始ポイントが変更された時にサムネイル画像を同期します
    /// </summary>
    /// <param name="startPoint">新しい開始ポイント</param>
    private async Task SynchronizeStartPointImage(TimeSpan? startPoint)
    {
        var point = startPoint ?? TimeSpan.Zero;
        StartPointImagePath = await ThumbnailCreator.CreateThumbnailAsync(FilePath, point);
    }

    /// <summary>
    /// 終了ポイントが変更された時にサムネイル画像を同期します
    /// </summary>
    /// <param name="endPoint">新しい終了ポイント</param>
    private async Task SynchronizeEndPointImage(TimeSpan? endPoint)
    {
        if (endPoint is TimeSpan point)
        {
            EndPointImagePath = await ThumbnailCreator.CreateThumbnailAsync(FilePath, point);
        }
        else
        {
            EndPointImagePath = await ThumbnailCreator.CreateLastThumbnailAsync(FilePath);
        }
    }

    /// <summary>
    /// あるアイテム情報から別のアイテム情報に編集プロパティをコピーします
    /// </summary>
    /// <param name="source">コピー元のアイテム情報</param>
    /// <param name="destination">コピー先のアイテム情報</param>
    /// <remarks>
    /// このメソッドは、編集に関連するプロパティのみをコピーします。
    /// ファイルパスやGUIDなどの識別情報はコピーされません。
    /// </remarks>
    public static void Copy(ItemInfo source, ItemInfo destination)
    {
        destination.StartPoint = source.StartPoint;
        destination.EndPoint = source.EndPoint;
        destination.StartPointImagePath = source.StartPointImagePath;
        destination.EndPointImagePath = source.EndPointImagePath;
        destination.CropRect = source.CropRect;
        destination.ResizedWidth = source.ResizedWidth;
        destination.ResizedHeight = source.ResizedHeight;
        destination.Rotation = source.Rotation;
        destination.PlaybackSpeed = source.PlaybackSpeed;
        destination.FrameRate = source.FrameRate;
        destination.Codec = source.Codec;
        destination.IsAudioDisabled = source.IsAudioDisabled;
        destination.FramesPerSecond = source.FramesPerSecond;
        destination.TotalFrames = source.TotalFrames;
        destination.Quality = source.Quality;
    }
}
