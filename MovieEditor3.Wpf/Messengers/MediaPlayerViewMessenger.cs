using System.Windows;

using MovieEditor3.Wpf.Views;

using Microsoft.Xaml.Behaviors;
using System.Reactive;

namespace MovieEditor3.Wpf.Messengers;

internal class SetupMediaPlayerAction : TriggerAction<MediaPlayerView>
{
    protected override void Invoke(object parameter)
    {
        if (parameter is DependencyPropertyChangedEventArgs e
        && e.NewValue is SetupMediaPlayerRequest request)
        {
            AssociatedObject.Setup(request.DurationChanged, request.StoryUpdated, request.StoryEnded, request.SliderValueChanged);
        }
    }
}

internal class LoadMediaAction : TriggerAction<MediaPlayerView>
{
    protected override void Invoke(object parameter)
    {
        if (parameter is DependencyPropertyChangedEventArgs e
        && e.NewValue is LoadMediaRequest request)
        {
            AssociatedObject.LoadMedia(request.FilePath);
        }
    }
}

internal class PlayAction : TriggerAction<MediaPlayerView>
{
    protected override void Invoke(object parameter)
    {
        if (parameter is DependencyPropertyChangedEventArgs e
        && e.NewValue is MediaActionRequest)
        {
            AssociatedObject.Play();
        }
    }
}

internal class PauseAction : TriggerAction<MediaPlayerView>
{
    protected override void Invoke(object parameter)
    {
        if (parameter is DependencyPropertyChangedEventArgs e
        && e.NewValue is MediaActionRequest)
        {
            AssociatedObject.Pause();
        }
    }
}

internal class MuteAction : TriggerAction<MediaPlayerView>
{
    protected override void Invoke(object parameter)
    {
        if (parameter is DependencyPropertyChangedEventArgs e
        && e.NewValue is MuteRequest request)
        {
            AssociatedObject.ToggleMute(request.IsMuted);
        }
    }
}

internal class SeekAction : TriggerAction<MediaPlayerView>
{
    protected override void Invoke(object parameter)
    {
        if (parameter is DependencyPropertyChangedEventArgs e
        && e.NewValue is SeekRequest request)
        {
            AssociatedObject.Seek(request.Offset);
        }
    }
}

internal class ChangeSliderValueAction : TriggerAction<MediaPlayerView>
{
    protected override void Invoke(object parameter)
    {
        if (parameter is DependencyPropertyChangedEventArgs e
        && e.NewValue is SeekRequest request)
        {
            AssociatedObject.ChangeSliderValue(request.Offset);
        }
    }
}

internal class SetupMediaPlayerRequest
{
    public required IObserver<TimeSpan> DurationChanged { get; init; }
    public required IObserver<TimeSpan> StoryUpdated { get; init; }
    public required IObserver<Unit> StoryEnded { get; init; }
    public required IObserver<TimeSpan> SliderValueChanged { get; init; }
}

internal class LoadMediaRequest
{
    public required string FilePath { get; init; }
}

internal class MediaActionRequest { }

internal class MuteRequest
{
    public required bool IsMuted { get; init; }
}

internal class SeekRequest
{
    public required TimeSpan Offset { get; init; }
}
