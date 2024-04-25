using Microsoft.Graphics.Canvas;

namespace MrCapitalQ.FollowAlong.Core.Capture;

public interface ICaptureSessionAdapter
{
    event EventHandler<FrameArrivedEventArgs>? FrameArrived;
    event EventHandler? Recreated;

    CanvasDevice? CanvasDevice { get; }

    void Start(IDisplayCaptureItem captureItem);
    void Stop();
}