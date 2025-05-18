using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using MovieEditor3.Wpf.Messengers;

using Reactive.Bindings;

namespace MovieEditor3.Wpf.ViewModels;

/// <summary>
/// メディアプレーヤーのビューモデルクラス
/// </summary>
internal partial class MediaPlayerViewModel : ObservableObject
{
    /// <summary>
    /// クロップ領域のビューモデル
    /// </summary>
    public CropAreaViewModel CropAreaContext { get; } = new();

    /// <summary>
    /// メディアが再生中かどうかを示す値
    /// </summary>
    [ObservableProperty] private bool _isPlaying = false;

    /// <summary>
    /// メディアの再生が完了したかどうかを示す値
    /// </summary>
    [ObservableProperty] private bool _isStoryCompleted = false;

    /// <summary>
    /// メディアの音声がミュートされているかどうかを示す値
    /// </summary>
    [ObservableProperty] private bool _isMute = false;

    /// <summary>
    /// 現在の再生位置
    /// </summary>
    [ObservableProperty] private TimeSpan _currentTime = TimeSpan.Zero;

    /// <summary>
    /// メディアの総再生時間
    /// </summary>
    [ObservableProperty] private TimeSpan _duration = TimeSpan.Zero;

    /// <summary>
    /// 編集の開始ポイント
    /// </summary>
    [ObservableProperty] private TimeSpan _startPoint = TimeSpan.Zero;

    /// <summary>
    /// 編集の終了ポイント
    /// </summary>
    [ObservableProperty] private TimeSpan _endPoint = TimeSpan.Zero;

    /// <summary>
    /// クロップ領域が表示されているかどうかを示す値
    /// </summary>
    [ObservableProperty] private bool _isCropAreaVisible = false;

    /// <summary>
    /// スライダー値変更時の処理
    /// </summary>
    /// <param name="e">値変更イベント引数</param>
    [RelayCommand]
    private void SliderValueChanged(RoutedPropertyChangedEventArgs<double> e)
    {
        if (false == IsPlaying)
        {
            // System.Diagnostics.Debug.WriteLine($"{e.NewValue}");
            Seek(TimeSpan.FromMilliseconds(Math.Round(e.NewValue)));
        }
    }

    /// <summary>
    /// メディアの再生/一時停止を切り替えます
    /// </summary>
    /// <param name="isPlaying">再生する場合はtrue、一時停止する場合はfalse</param>
    [RelayCommand]
    private void ToggleMedia(bool isPlaying)
    {
        if (isPlaying)
        {
            Play();
        }
        else
        {
            Pause();
        }
    }

    /// <summary>
    /// 現在の状態をもとにメディアの再生/一時停止を切り替えます
    /// </summary>
    [RelayCommand]
    private void ToggleMediaAuto()
    {
        IsPlaying = !IsPlaying;
        ToggleMedia(IsPlaying);
    }

    /// <summary>
    /// メディアの先頭にスキップします
    /// </summary>
    [RelayCommand]
    private void SkipPrevious()
    {
        Seek(TimeSpan.Zero);
    }

    /// <summary>
    /// メディアを最初から再生します
    /// </summary>
    [RelayCommand]
    private void Replay()
    {
        IsStoryCompleted = false;
        IsPlaying = true;
        Seek(TimeSpan.Zero);
    }

    /// <summary>
    /// メディアの音声ミュート状態を切り替えます
    /// </summary>
    /// <param name="isMute">ミュートする場合はtrue、ミュート解除する場合はfalse</param>
    [RelayCommand]
    private void ToggleMute(bool isMute)
    {
        MuteReq = new MuteRequest { IsMuted = isMute };
    }


    [ObservableProperty] private SetupMediaPlayerRequest? _setupReq = null;
    [ObservableProperty] private LoadMediaRequest? _loadMediaReq = null;
    [ObservableProperty] private MediaActionRequest? _playReq = null;
    [ObservableProperty] private MediaActionRequest? _pauseReq = null;
    [ObservableProperty] private MuteRequest? _muteReq = null;
    [ObservableProperty] private SeekRequest? _seekReq = null;

    /// <summary>
    /// 総再生時間変更通知用のサブジェクト
    /// </summary>
    private readonly Subject<TimeSpan> _durationChanged = new();

    /// <summary>
    /// 再生位置更新通知用のサブジェクト
    /// </summary>
    private readonly Subject<TimeSpan> _storyUpdated = new();

    /// <summary>
    /// 再生終了通知用のサブジェクト
    /// </summary>
    private readonly Subject<Unit> _storyEnded = new();

    public MediaPlayerViewModel()
    {
        _durationChanged.Subscribe(duration => Duration = duration);
        _storyUpdated.Where(_ => IsPlaying).Subscribe(currentTime => CurrentTime = currentTime);
        // _storyUpdated.Subscribe(currentTime => SliderValue = currentTime.TotalMilliseconds);
        _storyEnded.Subscribe(_ => OnStoryEnded());

        SetupReq = new SetupMediaPlayerRequest { DurationChanged = _durationChanged, StoryUpdated = _storyUpdated, StoryEnded = _storyEnded };
    }

    /// <summary>
    /// メディアを読み込みます
    /// </summary>
    /// <param name="itemInfo">読み込むメディアのアイテム情報</param>
    public void LoadMedia(ItemInfo itemInfo)
    {
        IsPlaying = false;
        IsStoryCompleted = false;
        CurrentTime = TimeSpan.Zero;

        LoadMediaReq = new LoadMediaRequest { FilePath = itemInfo.FilePath };
        CropAreaContext.LoadCropAreaInfo(itemInfo);
    }

    /// <summary>
    /// メディアを再生します
    /// </summary>
    public void Play() => PlayReq = new MediaActionRequest();

    /// <summary>
    /// メディアを一時停止します
    /// </summary>
    public void Pause() => PauseReq = new MediaActionRequest();

    /// <summary>
    /// 指定した位置にシークします
    /// </summary>
    /// <param name="offset">シーク先の位置</param>

    public void Seek(TimeSpan offset) => SeekReq = new SeekRequest { Offset = offset };

    /// <summary>
    /// クロップ領域の表示/非表示を切り替えます
    /// </summary>
    /// <param name="isibility">表示する場合はtrue、非表示にする場合はfalse</param>
    public void ToggleCropAreaVisibility(bool isibility)
    {
        IsCropAreaVisible = isibility;
    }

    /// <summary>
    /// 再生終了時の処理
    /// </summary>
    private void OnStoryEnded()
    {
        IsStoryCompleted = true;
        IsPlaying = false;
    }

}
