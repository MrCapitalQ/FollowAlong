using System.Drawing;

namespace MrCapitalQ.FollowAlong.Core.Capture;

public interface IDisplayCaptureItem
{
    event EventHandler? Closed;

    string DisplayName { get; }
    Rectangle OuterBounds { get; }
}