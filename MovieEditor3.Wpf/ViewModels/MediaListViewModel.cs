using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Controls;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Reactive.Bindings;

namespace MovieEditor3.Wpf.ViewModels;

internal partial class MediaListViewModel : ObservableObject
{
    /// <summary> メディアアイテムリスト </summary>
    public ObservableCollection<ItemInfo> MediaItems { get; } = [];

    /// <summary>
    /// 全選択状態を管理するプロパティ（選択・非選択が混在する場合はnull）
    /// </summary>
    [ObservableProperty] private bool? _isAllSelected = false;

    /// <summary>
    /// リストが空かどうかを監視するReactiveプロパティ
    /// </summary>
    public IReadOnlyReactiveProperty<bool> IsEmpty => _isEmpty;

    /// <summary>
    /// 選択中のアイテムを監視するReactiveプロパティ
    /// </summary>
    public IReadOnlyReactiveProperty<ItemInfo?> IsSelected => _isSelected;

    /// <summary>
    /// 選択状態が変更されたときのコマンド
    /// </summary>
    /// <param name="itemInfo">選択されたアイテム</param>
    [RelayCommand]
    private void SelectionChanged(ItemInfo? itemInfo)
    {
        _isSelected.Value = itemInfo;

        if (itemInfo is null) return;

        System.Diagnostics.Debug.WriteLine($"{itemInfo.OriginalMediaInfo.FileNameWithoutExtension}");
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
        MediaItems.Remove(itemInfo);
    }

    /// <summary>
    /// 指定のアイテムを複製するコマンド
    /// </summary>
    [RelayCommand]
    private void Clone(ItemInfo itemInfo)
    {
        var cloneItem = new ItemInfo();
        cloneItem.CopyInfoFrom(itemInfo);
        AddItem(cloneItem);
    }

    /// <summary>
    /// リストが空かどうかを監視するReactiveプロパティ
    /// </summary>
    private readonly ReactivePropertySlim<bool> _isEmpty = new(true);

    /// <summary>
    /// 選択中のアイテムを監視するReactiveプロパティ
    /// </summary>
    private readonly ReactivePropertySlim<ItemInfo?> _isSelected = new(null);

    public MediaListViewModel()
    {
        MediaItems.CollectionChanged += OnCollectionChanged;
    }

    /// <summary>
    /// 複数のファイルパスについて非同期でアイテムを生成し、リストに追加する
    /// </summary>
    /// <param name="items">ファイルパスの列挙</param>
    public async Task AddItemsAsync(IEnumerable<string> items)
    {
        // 並列処理用のワークスペース生成
        var workspace = items.ToDictionary(item => item, item => new ItemInfo());

        // 非同期で並列処理実行
        await Task.Run(() => CreateItems(workspace));

        // 生成したアイテムをリストに追加
        foreach (var result in workspace)
        {
            AddItem(result.Value);
        }
    }

    /// <summary>
    /// ファイルパスとItemInfoの辞書から各アイテムの情報を並列処理によって取得
    /// </summary>
    /// <param name="workspace">ファイルパスとItemInfoの辞書</param>
    private static void CreateItems(Dictionary<string, ItemInfo> workspace)
    {
        workspace.AsParallel()
        .ForAll(static source =>
        {
            try
            {
                source.Value.LoadInfo(source.Key);
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
            if (IsAllSelected is not null && IsAllSelected != itemInfo.IsSelected)
            {
                IsAllSelected = null;
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
