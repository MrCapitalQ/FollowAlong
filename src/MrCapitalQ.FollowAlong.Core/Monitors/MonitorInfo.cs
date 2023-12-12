using System.Numerics;
using Windows.Foundation;

namespace MrCapitalQ.FollowAlong.Core.Monitors
{
    public record MonitorInfo(bool IsPrimary,
        Vector2 ScreenSize,
        Rect MonitorArea,
        Rect WorkArea,
        string DeviceName,
        nint Hmon);
}
