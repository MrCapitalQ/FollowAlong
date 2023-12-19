using Microsoft.Graphics.Canvas;
using Windows.Graphics;

namespace MrCapitalQ.FollowAlong.Core.Capture
{
    public interface IBitmapFrameHandler
    {
        void Initialize(CanvasDevice canvasDevice, SizeInt32? size = null);
        void HandleFrame(CanvasBitmap canvasBitmap);
    }
}