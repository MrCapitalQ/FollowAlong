using Windows.Graphics;

namespace MrCapitalQ.FollowAlong.Infrastructure.Capture;

public class CaptureStartedEventArgs : EventArgs
{
    public CaptureStartedEventArgs(SizeInt32 size) => Size = size;

    public SizeInt32 Size { get; }
}
