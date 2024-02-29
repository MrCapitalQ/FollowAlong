using Microsoft.Graphics.Canvas;
using System;

namespace MrCapitalQ.FollowAlong.Core.Capture
{
    public interface ICaptureSessionAdapter
    {
        event EventHandler<FrameArrivedEventArgs>? FrameArrived;
        event EventHandler? Recreated;

        CanvasDevice? CanvasDevice { get; }

        void Start(IDisplayCaptureItem captureItem);
        void Stop();
    }
}