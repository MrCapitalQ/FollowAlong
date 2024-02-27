using System;
using Windows.Foundation;

namespace MrCapitalQ.FollowAlong.Core.Capture
{
    public interface IDisplayCaptureItem
    {
        event EventHandler? Closed;

        string DisplayName { get; }
        Rect OuterBounds { get; }
    }
}