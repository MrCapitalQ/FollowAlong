using Microsoft.Graphics.Canvas;
using Windows.Foundation;

namespace MrCapitalQ.FollowAlong.Core.Capture
{
    public interface IBitmapFrameHandler
    {
        void Initialize(CanvasDevice canvasDevice, Rect? contentArea = null);
        void HandleFrame(CanvasBitmap canvasBitmap);
        void Stop();
    }
}