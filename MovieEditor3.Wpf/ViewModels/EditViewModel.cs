using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MovieEditor3.Wpf.ViewModels;

/// <summary>
/// 動画編集画面のビューモデルクラス
/// </summary>
internal partial class EditViewModel : ObservableObject
{
    // private const int TAB_INDEX_MOVIE_EDIT = 0;

    /// <summary>
    /// クロップ編集タブのインデックス
    /// </summary>
    private const int TAB_INDEX_CROP_EDIT = 1;

    /// <summary>
    /// メディアプレーヤーのビューモデル
    /// </summary>
    public MediaPlayerViewModel MediaPlayerViewContext { get; } = new();

    /// <summary>
    /// 動画編集のビューモデル
    /// </summary>
    public MovieEditViewModel MovieEditViewContext { get; } = new();

    /// <summary>
    /// クロップ編集のビューモデル
    /// </summary>
    public CropEditViewModel CropEditViewContext { get; } = new();

    /// <summary>
    /// 選択されたアイテム情報
    /// </summary>
    [ObservableProperty] private ItemInfo? _selectedItem = null;

    /// <summary>
    /// タブ選択変更時の処理
    /// </summary>
    /// <param name="index">選択されたタブのインデックス</param>
    [RelayCommand]
    private void TabSelectionChanged(int index)
    {
        if (TAB_INDEX_CROP_EDIT == index)
        {
            MediaPlayerViewContext.ToggleCropAreaVisibility(true);
        }
        else
        {
            MediaPlayerViewContext.ToggleCropAreaVisibility(false);
        }
    }

    public EditViewModel()
    {
        // イベント設定
        MovieEditViewContext.StartPointChangeRequested.Subscribe(_ => ChangeStartPoint());
        MovieEditViewContext.EndPointChangeRequested.Subscribe(_ => ChangeEndPoint());
        CropEditViewContext.OnClearCropAreaRequested.Subscribe(_ => MediaPlayerViewContext.ClearCropArea());
    }

    /// <summary>
    /// アイテムを選択し、関連するビューモデルに情報を読み込みます
    /// </summary>
    /// <param name="itemInfo">選択するアイテム情報</param>
    public void SelectItem(ItemInfo? itemInfo)
    {
        SelectedItem = itemInfo;
        if (itemInfo is not null)
        {
            MediaPlayerViewContext.LoadMedia(itemInfo);
            MovieEditViewContext.LoadMovieEditInfo(itemInfo);
            CropEditViewContext.LoadCropEditInfo(itemInfo);
        }
    }

    /// <summary>
    /// 開始ポイントを現在の再生位置に変更します
    /// </summary>
    private void ChangeStartPoint()
    {
        if (SelectedItem is not null)
        {
            SelectedItem.StartPoint = MediaPlayerViewContext.CurrentTime;
        }

    }

    /// <summary>
    /// 終了ポイントを現在の再生位置に変更します
    /// </summary>
    private void ChangeEndPoint()
    {
        if (SelectedItem is not null)
        {
            SelectedItem.EndPoint = MediaPlayerViewContext.CurrentTime;
        }
    }
}
