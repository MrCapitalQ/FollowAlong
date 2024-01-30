using MrCapitalQ.FollowAlong.Core.Monitors;
using Windows.Graphics.Capture;

namespace MrCapitalQ.FollowAlong.Core.Capture
{
    public record MonitorCaptureItem(GraphicsCaptureItem GraphicsCaptureItem, MonitorInfo MonitorInfo);
}
