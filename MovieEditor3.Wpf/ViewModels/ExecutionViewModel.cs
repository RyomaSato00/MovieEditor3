using System.Reactive;
using System.Reactive.Subjects;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Reactive.Bindings;

namespace MovieEditor3.Wpf.ViewModels;

/// <summary>
/// 実行操作に関するビューモデルクラス
/// </summary>
internal partial class ExecutionViewModel : ObservableObject
{
    /// <summary>
    /// 実行操作が有効かどうかを示すリアクティブプロパティ
    /// </summary>
    public ReactivePropertySlim<bool> IsExecutionEnabled { get; } = new(false);

    /// <summary>
    /// 圧縮処理を実行するコマンド
    /// </summary>
    public ReactiveCommand CompCommand { get; }

    /// <summary>
    /// 画像生成処理を実行するコマンド
    /// </summary>
    public ReactiveCommand GenerateImagesCommand { get; }

    public ExecutionViewModel()
    {
        // IsExecutionEnabledがtrueのときのみ実行可能なコマンド
        CompCommand = IsExecutionEnabled.ToReactiveCommand();
        GenerateImagesCommand = IsExecutionEnabled.ToReactiveCommand();
    }

}
