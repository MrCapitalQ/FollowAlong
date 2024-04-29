using Windows.Graphics;

namespace MrCapitalQ.FollowAlong.Core.Capture;

public class CaptureStartedEventArgs : EventArgs
{
    public CaptureStartedEventArgs(SizeInt32 size) => Size = size;

    public SizeInt32 Size { get; }
}
