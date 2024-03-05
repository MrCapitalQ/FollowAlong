using Microsoft.Graphics.Canvas;
using System;

namespace MrCapitalQ.FollowAlong.Core.Capture
{
    public class FrameArrivedEventArgs : EventArgs
    {
        public FrameArrivedEventArgs(CanvasBitmap bitmap) => Bitmap = bitmap;

        public CanvasBitmap Bitmap { get; }
    }
}
