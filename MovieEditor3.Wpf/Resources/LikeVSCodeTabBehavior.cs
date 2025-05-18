

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace MovieEditor3.Wpf.Resources;

public static class LikeVSCodeTabBehavior
{
    /// <summary> TabItemのヘッダエリア（クリックするエリア） </summary>
    public static readonly DependencyProperty ClickAreaProperty =
    DependencyProperty.RegisterAttached("ClickArea", typeof(FrameworkElement), typeof(LikeVSCodeTabBehavior), new PropertyMetadata(null, OnClickAreaAttached));
    public static FrameworkElement GetClickArea(DependencyObject sender) => (FrameworkElement)sender.GetValue(ClickAreaProperty);
    public static void SetClickArea(DependencyObject sender, FrameworkElement value) => sender.SetValue(ClickAreaProperty, value);

    /// <summary> 対象のTabItem </summary>
    public static readonly DependencyProperty SelfProperty =
    DependencyProperty.RegisterAttached("Self", typeof(TabItem), typeof(LikeVSCodeTabBehavior), new PropertyMetadata(null));
    public static TabItem GetSelf(DependencyObject sender) => (TabItem)sender.GetValue(SelfProperty);
    public static void SetSelf(DependencyObject sender, TabItem value) => sender.SetValue(SelfProperty, value);

    /// <summary> 対象のTabItemを所有するTabControl </summary>
    public static readonly DependencyProperty ParentProperty =
    DependencyProperty.RegisterAttached("Parent", typeof(TabControl), typeof(LikeVSCodeTabBehavior), new PropertyMetadata(null));
    public static TabControl GetParent(DependencyObject sender) => (TabControl)sender.GetValue(ParentProperty);
    public static void SetParent(DependencyObject sender, TabControl value) => sender.SetValue(ParentProperty, value);


    private static void OnClickAreaAttached(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is not FrameworkElement tabItem) return;

        if (e.NewValue is FrameworkElement frameworkElement)
        {
            frameworkElement.MouseLeftButtonDown += ClickArea_MouseLeftButtonDown;
        }
    }

    private static void ClickArea_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is not DependencyObject dependencyObject) return;

        if (GetSelf(dependencyObject) is not TabItem self) return;

        if (GetParent(dependencyObject) is not TabControl parent) return;

        // このTabItemを選択中にもう一度選択した？
        if (self.IsSelected)
        {
            // 選択解除および無選択状態
            parent.SelectedIndex = -1;
            self.IsSelected = false;
        }
        // このTabItemはまだ選択されていない？
        else
        {
            // このTabItemを選択状態にする
            parent.SelectedIndex = self.TabIndex;
            self.IsSelected = true;
        }

        e.Handled = true;
    }
}
