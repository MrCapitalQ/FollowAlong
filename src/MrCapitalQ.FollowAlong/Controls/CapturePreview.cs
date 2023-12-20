using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Composition;
using Microsoft.UI;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Hosting;
using MrCapitalQ.FollowAlong.Core.Capture;
using System.Numerics;
using Windows.Foundation;

namespace MrCapitalQ.FollowAlong.Controls
{
    public sealed class CapturePreview : Control, IBitmapFrameHandler
    {
        private Size _surfaceSize;
        private CompositionDrawingSurface? _surface;

        public CapturePreview() => DefaultStyleKey = typeof(CapturePreview);

        public void Initialize(CanvasDevice canvasDevice, Size? size = null)
        {
            if (size.HasValue)
                _surfaceSize = size.Value;

            var compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
            var compositionGraphicsDevice = CanvasComposition.CreateCompositionGraphicsDevice(compositor, canvasDevice);

            _surface = compositionGraphicsDevice.CreateDrawingSurface(_surfaceSize,
                 Microsoft.Graphics.DirectX.DirectXPixelFormat.B8G8R8A8UIntNormalized,
                 Microsoft.Graphics.DirectX.DirectXAlphaMode.Premultiplied);

            var brush = compositor.CreateSurfaceBrush(_surface);
            brush.Scale = new Vector2((float)(1 / XamlRoot.RasterizationScale));
            brush.Stretch = CompositionStretch.None;

            var visual = compositor.CreateSpriteVisual();
            visual.RelativeSizeAdjustment = Vector2.One;
            visual.Brush = brush;

            ElementCompositionPreview.SetElementChildVisual(this, visual);
        }

        public void HandleFrame(CanvasBitmap canvasBitmap)
        {
            using var session = CanvasComposition.CreateDrawingSession(_surface);
            session.Clear(Colors.Transparent);
            session.DrawImage(canvasBitmap);
        }
    }
}
