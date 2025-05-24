using System.IO;
using System.Reactive;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MovieEditor3.Wpf.Views;

/// <summary>
/// Interaction logic for MediaPlayerView.xaml
/// </summary>
public partial class MediaPlayerView : UserControl
{
    /// <summary> 動画のStoryboard </summary>
    private readonly Storyboard _story;

    /// <summary>
    /// 総再生時間変更通知用のオブザーバー
    /// </summary>
    private IObserver<TimeSpan>? _durationChanged = null;

    /// <summary>
    /// 再生位置更新通知用のオブザーバー
    /// </summary>
    private IObserver<TimeSpan>? _storyUpdated = null;

    /// <summary>
    /// 再生終了通知用のオブザーバー
    /// </summary>
    private IObserver<Unit>? _storyEnded = null;

    /// <summary>
    /// スライダーの値変更通知用のオブザーバー
    /// </summary>
    private IObserver<TimeSpan>? _sliderValueChanged = null;

    public MediaPlayerView()
    {
        InitializeComponent();

        _story = (Storyboard)FindResource("MediaStory");
    }

    /// <summary>
    /// メディアプレーヤービューをセットアップします
    /// </summary>
    /// <param name="durationChanged">再生時間変更通知用のオブザーバー</param>
    /// <param name="storyUpdated">再生位置更新通知用のオブザーバー</param>
    /// <param name="storyEnded">再生終了通知用のオブザーバー</param>
    public void Setup(IObserver<TimeSpan> durationChanged, IObserver<TimeSpan> storyUpdated, IObserver<Unit> storyEnded, IObserver<TimeSpan> sliderValueChanged)
    {
        _durationChanged = durationChanged;
        _storyUpdated = storyUpdated;
        _storyEnded = storyEnded;
        _sliderValueChanged = sliderValueChanged;
    }

    /// <summary>
    /// メディアを読み込みます
    /// </summary>
    /// <param name="filePath">メディアファイルのパス</param>
    public void LoadMedia(string? filePath)
    {
        var timeline = (MediaTimeline)_story.Children[0];
        if (filePath is null || false == File.Exists(filePath))
        {
            timeline.Source = null;
        }
        else
        {
            timeline.Source = new Uri(filePath);

            _story.Begin();

            _story.Pause();
        }
    }

    /// <summary>
    /// メディアを再生します
    /// </summary>
    public void Play()
    {
        _story.Resume();
    }

    /// <summary>
    /// メディアを一時停止します
    /// </summary>
    public void Pause()
    {
        _story.Pause();
    }

    /// <summary>
    /// メディアの音声ミュート状態を切り替えます
    /// </summary>
    /// <param name="isMuted">ミュートする場合はtrue、ミュート解除する場合はfalse</param>
    public void ToggleMute(bool isMuted)
    {
        MediaPlayer.IsMuted = isMuted;
    }

    /// <summary>
    /// 指定した位置にシークします
    /// </summary>
    /// <param name="offset">シーク先の位置</param>
    public void Seek(TimeSpan offset)
    {
        _story.Seek(offset);
    }

    /// <summary>
    /// スライダーの値を変更します
    /// </summary>
    /// <param name="value"></param>
    public void ChangeSliderValue(TimeSpan value)
    {
        PositionSlider.Value = value.TotalMilliseconds;
    }

    /// <summary>
    /// 動画更新時処理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Timeline_CurrentTimeInvalidated(object sender, EventArgs e)
    {
        if (sender is not MediaClock mediaClock) return;

        TimeSpan currentTime = mediaClock.CurrentTime ?? TimeSpan.Zero;

        _storyUpdated?.OnNext(currentTime);
    }

    /// <summary>
    /// 動画Completed時処理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Timeline_Completed(object sender, EventArgs e)
    {
        _storyEnded?.OnNext(default);
    }

    /// <summary>
    /// 動画読み込み完了時に行う処理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MediaPlayer_MediaOpened(object sender, EventArgs e)
    {
        if (sender is not MediaElement mediaElement) return;

        TimeSpan duration = TimeSpan.Zero;
        if (mediaElement.NaturalDuration.HasTimeSpan)
        {
            duration = mediaElement.NaturalDuration.TimeSpan;
        }

        _durationChanged?.OnNext(duration);
    }

    /// <summary>
    /// スライダーの値が変更された時に行う処理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void PositionSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        _sliderValueChanged?.OnNext(TimeSpan.FromMilliseconds(Math.Round(e.NewValue)));
    }
}

