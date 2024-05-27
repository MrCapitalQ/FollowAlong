namespace MrCapitalQ.FollowAlong.Infrastructure.Capture;

public interface IBitmapCaptureService
{
    bool IsStarted { get; }

    event EventHandler<CaptureStartedEventArgs>? Started;
    event EventHandler? Stopped;

    void RegisterFrameHandler(IBitmapFrameHandler handler);
    void StartCapture(IDisplayCaptureItem captureItem);
    void StopCapture();
    void UnregisterFrameHandler(IBitmapFrameHandler handler);
}