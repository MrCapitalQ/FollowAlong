using Microsoft.Graphics.Canvas;

namespace MrCapitalQ.FollowAlong.Infrastructure.Capture;

public class FrameArrivedEventArgs(CanvasBitmap bitmap) : EventArgs
{
    public CanvasBitmap Bitmap { get; } = bitmap;
}
