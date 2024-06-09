using System.Drawing;

namespace MrCapitalQ.FollowAlong.Infrastructure.Capture;

public interface IDisplayCaptureItem
{
    event EventHandler? Closed;

    string DisplayName { get; }
    Rectangle OuterBounds { get; }
}