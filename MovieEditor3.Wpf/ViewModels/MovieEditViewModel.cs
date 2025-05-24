using System.Reactive;
using System.Reactive.Subjects;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MovieEditor3.Wpf.ViewModels;

/// <summary>
/// 動画編集機能のビューモデルクラス
/// </summary
internal partial class MovieEditViewModel : ObservableObject
{
    /// <summary>
    /// 開始ポイント変更要求を通知するObservable
    /// </summary>
    public IObservable<Unit> StartPointChangeRequested => _startPointChangeRequested;

    /// <summary>
    /// 終了ポイント変更要求を通知するObservable
    /// </summary>
    public IObservable<Unit> EndPointChangeRequested => _endPointChangeRequested;

    /// <summary>
    /// 動画編集プロパティ
    /// </summary>
    [ObservableProperty] private IMovieEditViewProperty? _property = null;


    /// <summary>
    /// 開始ポイントの変更を要求します
    /// </summary>
    [RelayCommand]
    private void ChangeStartPoint()
    {
        _startPointChangeRequested.OnNext(Unit.Default);
    }

    /// <summary>
    /// 終了ポイントの変更を要求します
    /// </summary>
    [RelayCommand]
    private void ChangeEndPoint()
    {
        _endPointChangeRequested.OnNext(Unit.Default);
    }

    /// <summary>
    /// 開始ポイントをクリアします
    /// </summary>
    [RelayCommand]
    private void ClearStartPoint()
    {
        if (Property is not null)
        {
            Property.StartPoint = null;
        }
    }

    /// <summary>
    /// 終了ポイントをクリアします
    /// </summary>
    [RelayCommand]
    private void ClearEndPoint()
    {
        if (Property is not null)
        {
            Property.EndPoint = null;
        }
    }

    /// <summary>
    /// 開始ポイント変更要求を通知するSubject
    /// </summary>
    private readonly Subject<Unit> _startPointChangeRequested = new();

    /// <summary>
    /// 終了ポイント変更要求を通知するSubject
    /// </summary>
    private readonly Subject<Unit> _endPointChangeRequested = new();

    /// <summary>
    /// 動画編集情報を読み込みます
    /// </summary>
    /// <param name="property">動画編集プロパティ</param>
    public void LoadMovieEditInfo(IMovieEditViewProperty? property)
    {
        Property = property;
    }
}
