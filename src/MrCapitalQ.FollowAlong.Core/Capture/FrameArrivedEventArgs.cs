using Microsoft.Graphics.Canvas;

namespace MrCapitalQ.FollowAlong.Core.Capture;

public class FrameArrivedEventArgs : EventArgs
{
    public FrameArrivedEventArgs(CanvasBitmap bitmap) => Bitmap = bitmap;

    public CanvasBitmap Bitmap { get; }
}
