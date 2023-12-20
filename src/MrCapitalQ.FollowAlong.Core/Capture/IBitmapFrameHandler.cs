using Microsoft.Graphics.Canvas;
using Windows.Foundation;

namespace MrCapitalQ.FollowAlong.Core.Capture
{
    public interface IBitmapFrameHandler
    {
        void Initialize(CanvasDevice canvasDevice, Size? size = null);
        void HandleFrame(CanvasBitmap canvasBitmap);
    }
}