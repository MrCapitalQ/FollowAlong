using System.Drawing;

namespace MrCapitalQ.FollowAlong.Infrastructure.Capture;

public class CaptureStartedEventArgs(Size size) : EventArgs
{
    public Size Size { get; } = size;
}
