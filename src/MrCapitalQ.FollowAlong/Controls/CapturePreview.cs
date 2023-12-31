using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Composition;
using Microsoft.UI;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Hosting;
using MrCapitalQ.FollowAlong.Core.Capture;
using MrCapitalQ.FollowAlong.Core.Tracking;
using System.Numerics;
using Windows.Foundation;

namespace MrCapitalQ.FollowAlong.Controls
{
    public sealed class CapturePreview : Control, IBitmapFrameHandler, ITrackingTransformTarget
    {
        private Size _surfaceSize;
        private CompositionDrawingSurface? _surface;
        private CompositionSurfaceBrush? _brush;
        private SpriteVisual? _visual;

        public CapturePreview() => DefaultStyleKey = typeof(CapturePreview);

        public CompositionSurfaceBrush? Brush => _brush;
        public Size ContentSize => _surfaceSize;
        public Size ViewportSize => ActualSize.ToSize();

        public void Initialize(CanvasDevice canvasDevice, Size? size = null)
        {
            if (size.HasValue)
                _surfaceSize = size.Value;

            var compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
            using var compositionGraphicsDevice = CanvasComposition.CreateCompositionGraphicsDevice(compositor, canvasDevice);

            _surface = compositionGraphicsDevice.CreateDrawingSurface(_surfaceSize,
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
            using var session = CanvasComposition.CreateDrawingSession(_surface);
            session.Clear(Colors.Transparent);

            ElementCompositionPreview.SetElementChildVisual(this, null);

            _visual?.Dispose();
            _visual = null;
            _brush?.Dispose();
            _brush = null;
        }
    }
}
