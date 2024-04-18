using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Composition;
using Microsoft.UI;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Hosting;
using MrCapitalQ.FollowAlong.Core.Capture;
using MrCapitalQ.FollowAlong.Core.Tracking;
using System.Numerics;
using Windows.Foundation;

namespace MrCapitalQ.FollowAlong.Controls;

public sealed class CapturePreview : Control, IBitmapFrameHandler, ITrackingTransformTarget
{
    public event EventHandler? ViewportSizeChanged;

    private Rect _contentArea;
    private CompositionDrawingSurface? _surface;
    private CompositionSurfaceBrush? _brush;
    private SpriteVisual? _visual;

    public CapturePreview()
    {
        DefaultStyleKey = typeof(CapturePreview);
        SizeChanged += CapturePreview_SizeChanged;
    }

    public Rect ContentArea => _contentArea;
    public Size ViewportSize => ActualSize.ToSize();

    public void Initialize(CanvasDevice canvasDevice, Rect? contentArea = null)
    {
        if (contentArea.HasValue)
            _contentArea = contentArea.Value;

        var compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
        using var compositionGraphicsDevice = CanvasComposition.CreateCompositionGraphicsDevice(compositor, canvasDevice);

        _surface = compositionGraphicsDevice.CreateDrawingSurface(new Size(_contentArea.Width, _contentArea.Height),
             Microsoft.Graphics.DirectX.DirectXPixelFormat.B8G8R8A8UIntNormalized,
             Microsoft.Graphics.DirectX.DirectXAlphaMode.Premultiplied);

        _brush = compositor.CreateSurfaceBrush(_surface);
        _brush.Stretch = CompositionStretch.None;

        _visual = compositor.CreateSpriteVisual();
        _visual.RelativeSizeAdjustment = Vector2.One;
        _visual.Brush = _brush;

        ElementCompositionPreview.SetElementChildVisual(this, _visual);
    }

    public void HandleFrame(CanvasBitmap canvasBitmap)
    {
        using var session = CanvasComposition.CreateDrawingSession(_surface);
        session.Clear(Colors.Transparent);
        session.DrawImage(canvasBitmap);
    }

    public void Stop()
    {
        if (_surface is not null)
        {
            try
            {
                using var session = CanvasComposition.CreateDrawingSession(_surface);
                session.Clear(Colors.Transparent);
            }
            catch (Exception ex) when (ex is ObjectDisposedException)
            {
                _surface = null;
            }
        }

        if (_visual is not null)
        {
            ElementCompositionPreview.SetElementChildVisual(this, null);

            _visual?.Dispose();
            _visual = null;
        }

        _brush?.Dispose();
        _brush = null;
    }

    public void SetCenterPoint(Vector2 centerPoint)
    {
        if (_brush is null)
            return;

        _brush.CenterPoint = centerPoint;
    }

    public void SetScale(float scale)
    {
        if (_brush is null)
            return;

        _brush.Scale = new Vector2(scale);
    }

    public void SetOffset(Vector2 offset)
    {
        if (_brush is null)
            return;

        _brush.Offset = offset;
    }

    private void CapturePreview_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        var raiseEvent = ViewportSizeChanged;
        raiseEvent?.Invoke(this, new());
    }
}
