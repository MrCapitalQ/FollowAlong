using Microsoft.Graphics.Canvas;
using System.Drawing;

namespace MrCapitalQ.FollowAlong.Infrastructure.Capture;

public interface IBitmapFrameHandler
{
    void Initialize(CanvasDevice canvasDevice, Rectangle? contentArea = null);
    void HandleFrame(CanvasBitmap canvasBitmap);
    void Stop();
}