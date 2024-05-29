using MrCapitalQ.FollowAlong.Infrastructure.Capture;

namespace MrCapitalQ.FollowAlong.Messages;

public class StartCapture
{
    private static readonly StartCapture s_emptyInstance = new();

    private StartCapture() { }

    public StartCapture(IDisplayCaptureItem captureItem) => CaptureItem = captureItem;

    public static StartCapture Empty => s_emptyInstance;

    public IDisplayCaptureItem? CaptureItem { get; }

}
