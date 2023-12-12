using Microsoft.Graphics.Canvas;

namespace MrCapitalQ.FollowAlong.Core.Capture
{
    public interface IBitmapFrameHandler
    {
        void HandleFrame(CanvasBitmap canvasBitmap);
    }
}