using Windows.Foundation;

namespace MrCapitalQ.FollowAlong.Infrastructure.Capture;

public interface IDisplayCaptureItem
{
    event EventHandler? Closed;

    string DisplayName { get; }
    Rect OuterBounds { get; }
}