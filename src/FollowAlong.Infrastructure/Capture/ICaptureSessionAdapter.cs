using Microsoft.Graphics.Canvas;
using MrCapitalQ.FollowAlong.Core.Capture;

namespace MrCapitalQ.FollowAlong.Infrastructure.Capture;

public interface ICaptureSessionAdapter
{
    event EventHandler<FrameArrivedEventArgs>? FrameArrived;
    event EventHandler? Recreated;

    CanvasDevice? CanvasDevice { get; }

    void Start(IDisplayCaptureItem captureItem);
    void Stop();
}