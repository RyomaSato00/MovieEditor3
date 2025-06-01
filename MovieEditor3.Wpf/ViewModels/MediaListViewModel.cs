using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using MaterialDesignThemes.Wpf;

using MovieEditor3.Wpf.Messengers;
using MovieEditor3.Wpf.Programs;
using MovieEditor3.Wpf.Views;

using Reactive.Bindings;

namespace MovieEditor3.Wpf.ViewModels;

internal partial class MediaListViewModel : ObservableObject
{
    /// <summary> メディアアイテムリスト </summary>
    public ObservableCollection<ItemInfo> MediaItems { get; } = [];

    /// <summary> エンプティステートのViewModel </summary>
    public EmptyStateViewModel EmptyStateViewContext { get; }

    /// <summary>
    /// 全選択状態を管理するプロパティ（選択・非選択が混在する場合はnull）
    /// </summary>
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
    /// 現在DataGridで選択されているアイテムのリスト
    /// </summary>
    public IReadOnlyList<ItemInfo> SelectedItems => _selectedItems;

    [ObservableProperty] private ShowDialogRequest? _showWaitingDialogReq = null;

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
    /// <param name="items">選択されたアイテム</param>
    [RelayCommand]
    private void SelectionChanged(System.Collections.IList items)
    {
        _selectedItems.Clear();

        if (0 < items.Count)
        {
            UpdateSelectedItem(items[0] as ItemInfo);

            foreach (var item in items)
            {
                if (item is ItemInfo itemInfo)
                {
                    _selectedItems.Add(itemInfo);
                }
            }
        }
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
        // 削除実行後に選択されるアイテムのインデックスを取得
        var afterIndex = MediaItems.IndexOf(itemInfo) - 1;
        afterIndex = afterIndex >= 0 ? afterIndex : 0;

        // アイテムを削除
        MediaItems.Remove(itemInfo);

        // 選択状態の変更
        if (0 < MediaItems.Count)
        {
            UpdateSelectedItem(MediaItems[afterIndex]);
        }
        else
        {
            UpdateSelectedItem(null);
        }
    }

    /// <summary>
    /// 指定のアイテムを複製するコマンド
    /// </summary>
    [RelayCommand]
    private void Clone(ItemInfo itemInfo)
    {
        var cloneItem = new ItemInfo(itemInfo);
        RegisterItemAt(cloneItem, MediaItems.IndexOf(itemInfo) + 1);
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
    /// DataGridで選択されたアイテムを保持するリスト
    /// </summary>
    private readonly List<ItemInfo> _selectedItems = [];

    public MediaListViewModel(UserSetting userSetting)
    {
        EmptyStateViewContext = new EmptyStateViewModel(userSetting);
        EmptyStateViewContext.MediaFilesOpened.Subscribe(files => _ = AddItemsAsync(files));
        MediaItems.CollectionChanged += OnCollectionChanged;
    }

    /// <summary>
    /// 現在の状態をユーザー設定に保存します。
    /// </summary>
    /// <param name="userSetting">保存先のユーザー設定インスタンス</param>
    public void SaveSetting(UserSetting userSetting)
    {
        EmptyStateViewContext.SaveSetting(userSetting);
    }

    /// <summary>
    /// 単一のファイルからアイテムを生成し、リストに追加する
    /// </summary>
    /// <param name="file">ファイルパス</param>
    /// <returns>非同期タスク</returns>
    public async Task AddItemAsync(string file)
    {
        var item = new ItemInfo(file);

        await Task.Run(item.LoadInfo);

        RegisterItem(item);
    }

    /// <summary>
    /// 複数のファイルパスについて非同期でアイテムを生成し、リストに追加する
    /// </summary>
    /// <param name="files">ファイルパスの列挙</param>
    public async Task AddItemsAsync(IEnumerable<string> files)
    {
        // 並列処理用のワークスペース生成
        var workspace = files.Select(file => new ItemInfo(file)).ToArray();

        ShowWaitingDialogReq = new ShowDialogRequest();

        // 非同期で並列処理実行
        await Task.Run(() => CreateItems(workspace));

        DialogHost.Close(nameof(MediaListView));

        // 生成したアイテムをリストに追加
        foreach (var item in workspace)
        {
            RegisterItem(item);
        }
    }

    /// <summary>
    /// DataGridで選択中のアイテムを更新する
    /// </summary>
    /// <param name="itemInfo">選択するアイテム、または選択解除の場合はnull</param>
    private void UpdateSelectedItem(ItemInfo? itemInfo)
    {
        _onSelected.Value = itemInfo;
    }

    /// <summary>
    /// 選択状態にあるアイテムをリストから削除する
    /// </summary>
    public void DeleteSelectedItems()
    {
        // 削除するアイテムの中で最も先頭にあるもののインデックスを取得（削除するアイテムがない場合は-1）
        var firstIndex = MediaItems
        .Where(item => item.IsSelected)
        .Select((item, index) => index)
        .DefaultIfEmpty(-1)
        .First();

        // 削除を実行した後に選択されるアイテムのインデックスを取得
        var afterIndex = firstIndex - 1 >= 0 ? firstIndex - 1 : 0;

        // 削除実行
        for (var i = MediaItems.Count - 1; i >= 0; i--)
        {
            if (MediaItems[i].IsSelected)
            {
                MediaItems.RemoveAt(i);
            }
        }

        // 選択状態の変更
        if (0 < MediaItems.Count)
        {
            UpdateSelectedItem(MediaItems[afterIndex]);
        }
        else
        {
            UpdateSelectedItem(null);
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
    /// アイテムをリストに登録する
    /// </summary>
    /// <param name="itemInfo"></param>
    private void RegisterItem(ItemInfo itemInfo)
    {
        itemInfo.PropertyChanged += OnPropertyChanged;
        MediaItems.Add(itemInfo);
    }

    /// <summary>
    /// アイテムをリストの指定位置に挿入する
    /// </summary>
    private void RegisterItemAt(ItemInfo itemInfo, int index)
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

                // 全選択状態を更新
                var isAllSelected = true;       // すべて選択状態？
                var isSelectedExist = false;        // 選択状態のアイテムが1つ以上存在する？
                foreach (var item in mediaItems)
                {
                    isAllSelected &= item.IsSelected;
                    isSelectedExist |= item.IsSelected;
                }

                // すべて選択状態
                if (isAllSelected && isSelectedExist)
                {
                    IsAllSelected.Value = true;
                }
                // すべて非選択状態
                else if (false == isAllSelected && false == isSelectedExist)
                {
                    IsAllSelected.Value = false;
                }
                // 選択状態が混在する場合
                else
                {
                    IsAllSelected.Value = null;
                }

            }
            break;

        default:
            break;
        }
    }
}
