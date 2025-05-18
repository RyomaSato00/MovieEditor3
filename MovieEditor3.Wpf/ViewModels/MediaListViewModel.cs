using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using MovieEditor3.Wpf.Programs;

using Reactive.Bindings;

namespace MovieEditor3.Wpf.ViewModels;

internal partial class MediaListViewModel : ObservableObject
{
    /// <summary> メディアアイテムリスト </summary>
    public ObservableCollection<ItemInfo> MediaItems { get; } = [];

    /// <summary>
    /// 全選択状態を管理するプロパティ（選択・非選択が混在する場合はnull）
    /// </summary>
    // [ObservableProperty] private bool? _isAllSelected = false;
    public ReactivePropertySlim<bool?> IsAllSelected { get; } = new(false);

    /// <summary>
    /// リストが空かどうかを監視するReactiveプロパティ
    /// </summary>
    public IReadOnlyReactiveProperty<bool> IsEmpty => _isEmpty;

    /// <summary>
    /// 選択中のアイテムを監視するReactiveプロパティ
    /// </summary>
    public IReadOnlyReactiveProperty<ItemInfo?> OnSelected => _onSelected;

    /// <summary>
    /// ドロップイベントハンドラ
    /// </summary>
    /// <param name="e">ドラッグイベント引数</param>
    [RelayCommand]
    private void Drop(DragEventArgs e)
    {
        // ドロップされたデータがファイルではない場合は何もしない
        if (false == e.Data.GetDataPresent(DataFormats.FileDrop)) return;

        // ドロップされたデータをファイルパスの配列として取得
        var files = (string[])e.Data.GetData(DataFormats.FileDrop);

        // 新しいファイルを追加
        _ = AddItemsAsync(files);
    }

    /// <summary>
    /// 選択状態が変更されたときのコマンド
    /// </summary>
    /// <param name="itemInfo">選択されたアイテム</param>
    [RelayCommand]
    private void SelectionChanged(ItemInfo? itemInfo)
    {
        _onSelected.Value = itemInfo;

        // System.Diagnostics.Debug.WriteLine($"{itemInfo.OriginalMediaInfo.FileNameWithoutExtension}");
    }

    /// <summary>
    /// 全アイテムを選択状態にするコマンド
    /// </summary>
    [RelayCommand]
    private void Checked()
    {
        foreach (var item in MediaItems)
        {
            item.IsSelected = true;
        }
    }

    /// <summary>
    /// 全アイテムを非選択状態にするコマンド
    /// </summary>
    [RelayCommand]
    private void Unchecked()
    {
        foreach (var item in MediaItems)
        {
            item.IsSelected = false;
        }
    }

    /// <summary>
    /// 指定のアイテムをリストから削除するコマンド
    /// </summary>
    [RelayCommand]
    private void Delete(ItemInfo itemInfo)
    {
        var afterIndex = MediaItems.IndexOf(itemInfo) - 1;
        afterIndex = afterIndex >= 0 ? afterIndex : 0;

        MediaItems.Remove(itemInfo);

        SelectionChanged(MediaItems[afterIndex]);
    }

    /// <summary>
    /// 指定のアイテムを複製するコマンド
    /// </summary>
    [RelayCommand]
    private void Clone(ItemInfo itemInfo)
    {
        var cloneItem = new ItemInfo(itemInfo, _userSetting);
        InsertItem(cloneItem, MediaItems.IndexOf(itemInfo) + 1);
    }

    /// <summary>
    /// リストが空かどうかを監視するReactiveプロパティ
    /// </summary>
    private readonly ReactivePropertySlim<bool> _isEmpty = new(true);

    /// <summary>
    /// 選択中のアイテムを監視するReactiveプロパティ
    /// </summary>
    private readonly ReactivePropertySlim<ItemInfo?> _onSelected = new(null);

    /// <summary>
    /// ユーザー設定
    /// </summary>
    private readonly UserSetting _userSetting;

    public MediaListViewModel(UserSetting userSetting)
    {
        _userSetting = userSetting;
        MediaItems.CollectionChanged += OnCollectionChanged;
    }

    /// <summary>
    /// 複数のファイルパスについて非同期でアイテムを生成し、リストに追加する
    /// </summary>
    /// <param name="files">ファイルパスの列挙</param>
    public async Task AddItemsAsync(IEnumerable<string> files)
    {
        // 並列処理用のワークスペース生成
        var workspace = files.Select(file => new ItemInfo(file, _userSetting)).ToArray();

        // 非同期で並列処理実行
        await Task.Run(() => CreateItems(workspace));

        // 生成したアイテムをリストに追加
        foreach (var item in workspace)
        {
            AddItem(item);
        }
    }

    /// <summary>
    /// 選択状態にあるアイテムをリストから削除する
    /// </summary>
    public void RemoveSelectedItems()
    {
        var selectedItems = MediaItems.Where(item => item.IsSelected);
        foreach (var item in selectedItems)
        {
            MediaItems.Remove(item);
        }
    }

    /// <summary>
    /// ファイルパスとItemInfoの辞書から各アイテムの情報を並列処理によって取得
    /// </summary>
    /// <param name="workspace">ファイルパスとItemInfoの辞書</param>
    private static void CreateItems(IEnumerable<ItemInfo> workspace)
    {
        workspace.AsParallel()
        .ForAll(static item =>
        {
            try
            {
                item.LoadInfo();
            }
            catch (ArgumentException e)
            {
                System.Diagnostics.Debug.WriteLine($"{e.Message}, param name:{e.ParamName}");
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
        });
    }

    /// <summary>
    ///  アイテムをリストに追加する
    /// </summary>
    /// <param name="itemInfo"></param>
    private void AddItem(ItemInfo itemInfo)
    {
        itemInfo.PropertyChanged += OnPropertyChanged;
        MediaItems.Add(itemInfo);
    }

    /// <summary>
    /// アイテムをリストの指定位置に挿入する
    /// </summary>
    private void InsertItem(ItemInfo itemInfo, int index)
    {
        itemInfo.PropertyChanged += OnPropertyChanged;
        MediaItems.Insert(index, itemInfo);
    }

    /// <summary>
    /// アイテムのプロパティ変更イベントハンドラ
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is not ItemInfo itemInfo) return;

        // 変更されたプロパティはIsSelected？
        if (e.PropertyName == nameof(ItemInfo.IsSelected))
        {
            // IsAllSelectedと異なる値に変更した場合のみIsAllSelectedをnullに変更
            if (IsAllSelected.Value is not null && IsAllSelected.Value != itemInfo.IsSelected)
            {
                IsAllSelected.Value = null;
            }
        }

    }

    /// <summary>
    /// コレクション変更時イベントハンドラ
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
        // コレクションの件数変更？
        case NotifyCollectionChangedAction.Add
            or NotifyCollectionChangedAction.Remove
            or NotifyCollectionChangedAction.Reset:

            if (sender is ObservableCollection<ItemInfo> mediaItems)
            {
                // コレクションの件数が0か0でないかを監視
                _isEmpty.Value = 0 == mediaItems.Count;
            }
            break;

        default:
            break;
        }
    }
}
