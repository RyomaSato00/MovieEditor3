using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MovieEditor3.Wpf.Views;

/// <summary>
/// Interaction logic for CropArea.xaml
/// </summary>
public partial class CropArea : UserControl
{
    /// <summary> クリッピング範囲（UserControl基準）の最小サイズ </summary>
    public static readonly Size MinimumSize = new(80, 80);

    /// <summary> 今ドラッグしているエッジまたはnull </summary>
    private FrameworkElement? _currentDragged = null;

    /// <summary> ドラッグしたエッジを基準にしたマウスクリック座標 </summary>
    private Vector _relativePosition = new(0, 0);

    /// <summary>
    /// クロップ領域変更通知用のオブザーバー
    /// </summary>
    private IObserver<Rect>? _cropAreaChanged = null;

    /// <summary>
    /// キャンバスサイズ変更通知用のオブザーバー
    /// </summary>
    private IObserver<Rect>? _canvasSizeChanged = null;

    public CropArea()
    {
        InitializeComponent();
    }

    /// <summary>
    /// クロップ領域コントロールをセットアップします
    /// </summary>
    /// <param name="cropAreaChanged">クロップ領域変更通知用のオブザーバー</param>
    /// <param name="canvasSizeChanged">キャンバスサイズ変更通知用のオブザーバー</param>
    public void Setup(IObserver<Rect> cropAreaChanged, IObserver<Rect> canvasSizeChanged)
    {
        _cropAreaChanged = cropAreaChanged;
        _canvasSizeChanged = canvasSizeChanged;
    }

    /// <summary>
    /// キャンバスのサイズ変更時処理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ParentCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (sender is not Canvas) return;

        BaseGeometry.Rect = new Rect(0, 0, e.NewSize.Width, e.NewSize.Height);

        if (0 != e.PreviousSize.Width && 0 != e.PreviousSize.Height)
        {
            var cropRect = new Rect
            {
                X = CropGeometry.Rect.X * e.NewSize.Width / e.PreviousSize.Width,
                Y = CropGeometry.Rect.Y * e.NewSize.Height / e.PreviousSize.Height,
                Width = CropGeometry.Rect.Width * e.NewSize.Width / e.PreviousSize.Width,
                Height = CropGeometry.Rect.Height * e.NewSize.Height / e.PreviousSize.Height,
            };

            UpdateArea(cropRect);
        }
        else
        {
            UpdateArea(new Rect(0, 0, e.NewSize.Width, e.NewSize.Height));
        }

        _canvasSizeChanged?.OnNext(new Rect(0, 0, e.NewSize.Width, e.NewSize.Height));
    }

    /// <summary>
    /// エッジ、キャンバス等のUI表示を更新する
    /// </summary>
    /// <param name="areaRect">クリッピング範囲（UserControl基準）</param>
    public void UpdateArea(Rect areaRect)
    {
        CropGeometry.Rect = areaRect;

        EdgeBorder.Data = new RectangleGeometry(areaRect);

        Canvas.SetLeft(TopLeftEdge, areaRect.TopLeft.X - TopLeftEdge.ActualWidth / 2);
        Canvas.SetTop(TopLeftEdge, areaRect.TopLeft.Y - TopLeftEdge.ActualHeight / 2);

        Canvas.SetLeft(TopRightEdge, areaRect.TopRight.X - TopRightEdge.ActualWidth / 2);
        Canvas.SetTop(TopRightEdge, areaRect.TopRight.Y - TopRightEdge.ActualHeight / 2);

        Canvas.SetLeft(BottomRightEdge, areaRect.BottomRight.X - BottomRightEdge.ActualWidth / 2);
        Canvas.SetTop(BottomRightEdge, areaRect.BottomRight.Y - BottomRightEdge.ActualHeight / 2);

        Canvas.SetLeft(BottomLeftEdge, areaRect.BottomLeft.X - BottomLeftEdge.ActualWidth / 2);
        Canvas.SetTop(BottomLeftEdge, areaRect.BottomLeft.Y - BottomLeftEdge.ActualHeight / 2);
    }

    /// <summary>
    /// エッジ上でマウスダウンしたときの処理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Edge_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is not FrameworkElement edge) return;

        // ドラッグするエッジを記憶
        _currentDragged = edge;

        // エッジ基準のマウス座標を取得
        _relativePosition = e.GetPosition(edge) - new Point(edge.ActualWidth / 2, edge.ActualHeight / 2);
    }

    /// <summary>
    /// エッジ上でマウスアップしたときの処理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Edge_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        // ドラッグ中オブジェクトを開放
        _currentDragged = null;
    }

    /// <summary>
    /// マウス移動時処理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Edge_MouseMove(object sender, MouseEventArgs e)
    {
        // UserControl基準のマウス座標を取得
        var current = e.GetPosition(this);

        // マウス座標変更通知
        // OnMouseMoved?.Invoke(current);

        // エッジをドラッグ中でなければここで終了
        if (_currentDragged is null) return;

        // マウスを左クリックしていないなら終了
        if (e.LeftButton == MouseButtonState.Released) return;

        // エッジの移動先を取得
        var destinationX = current.X - _relativePosition.X;
        var destinationY = current.Y - _relativePosition.Y;

        // エッジの移動先がCanvasの範囲外になる場合は境界に移動先を指定する
        if (destinationX < 0) destinationX = 0;
        if (destinationY < 0) destinationY = 0;
        if (destinationX > EdgeCanvas.ActualWidth) destinationX = EdgeCanvas.ActualWidth;
        if (destinationY > EdgeCanvas.ActualHeight) destinationY = EdgeCanvas.ActualHeight;

        // クリッピング範囲を計算する
        var areaRect = CalculateClip(destinationX, destinationY, _currentDragged.Name);

        // クリッピング範囲UI表示更新
        UpdateArea(areaRect);

        // クリッピング範囲変更通知
        _cropAreaChanged?.OnNext(areaRect);
        // OnClipRectChanged?.Invoke(areaRect);
    }

    /// <summary>
    /// クリッピング範囲（UserControl基準）を計算する
    /// </summary>
    /// <remarks>
    /// 動画の実範囲に合わせたクリッピング範囲は別途計算が必要
    /// </remarks>
    /// <param name="destinationX">エッジの移動先X座標</param>
    /// <param name="destinationY">エッジの移動先Y座標</param>
    /// <param name="edgeName">今ドラッグ中のエッジの名前</param>
    /// <returns></returns>
    private Rect CalculateClip(double destinationX, double destinationY, string edgeName)
    {
        double topLeftX;
        double topLeftY;
        double bottomRightX;
        double bottomRightY;
        Point topLeft;
        Point bottomRight;

        switch (edgeName)
        {
        case nameof(TopLeftEdge):

            bottomRightX = Canvas.GetLeft(BottomRightEdge) + BottomRightEdge.ActualWidth / 2;
            bottomRightY = Canvas.GetTop(BottomRightEdge) + BottomRightEdge.ActualHeight / 2;

            // クリッピング範囲が最小範囲を下回らないように設定
            if (bottomRightX - destinationX < MinimumSize.Width)
            {
                destinationX = bottomRightX - MinimumSize.Width;
            }

            if (bottomRightY - destinationY < MinimumSize.Height)
            {
                destinationY = bottomRightY - MinimumSize.Height;
            }

            topLeft = new Point(destinationX, destinationY);
            bottomRight = new Point(bottomRightX, bottomRightY);
            break;

        case nameof(TopRightEdge):

            topLeftX = Canvas.GetLeft(TopLeftEdge) + TopLeftEdge.ActualWidth / 2;
            bottomRightY = Canvas.GetTop(BottomRightEdge) + BottomRightEdge.ActualHeight / 2;

            // クリッピング範囲が最小範囲を下回らないように設定
            if (destinationX - topLeftX < MinimumSize.Width)
            {
                destinationX = topLeftX + MinimumSize.Width;
            }

            if (bottomRightY - destinationY < MinimumSize.Height)
            {
                destinationY = bottomRightY - MinimumSize.Height;
            }

            topLeft = new Point(topLeftX, destinationY);
            bottomRight = new Point(destinationX, bottomRightY);
            break;

        case nameof(BottomRightEdge):

            topLeftX = Canvas.GetLeft(TopLeftEdge) + TopLeftEdge.ActualWidth / 2;
            topLeftY = Canvas.GetTop(TopLeftEdge) + TopLeftEdge.ActualHeight / 2;

            // クリッピング範囲が最小範囲を下回らないように設定
            if (destinationX - topLeftX < MinimumSize.Width)
            {
                destinationX = topLeftX + MinimumSize.Width;
            }

            if (destinationY - topLeftY < MinimumSize.Height)
            {
                destinationY = topLeftY + MinimumSize.Height;
            }

            topLeft = new Point(topLeftX, topLeftY);
            bottomRight = new Point(destinationX, destinationY);
            break;

        case nameof(BottomLeftEdge):

            topLeftY = Canvas.GetTop(TopLeftEdge) + TopLeftEdge.ActualHeight / 2;
            bottomRightX = Canvas.GetLeft(BottomRightEdge) + BottomRightEdge.ActualWidth / 2;

            // クリッピング範囲が最小範囲を下回らないように設定
            if (bottomRightX - destinationX < MinimumSize.Width)
            {
                destinationX = bottomRightX - MinimumSize.Width;
            }

            if (destinationY - topLeftY < MinimumSize.Height)
            {
                destinationY = topLeftY + MinimumSize.Height;
            }

            topLeft = new Point(destinationX, topLeftY);
            bottomRight = new Point(bottomRightX, destinationY);
            break;

        default:
            break;
        }

        return new Rect(topLeft, bottomRight);
    }
}

