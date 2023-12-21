using Windows.Foundation;

namespace MrCapitalQ.FollowAlong.Core.Tracking
{
    public class PointerService
    {
        public Point? GetCurrentPosition() => PointerInterops.GetCursorPosition();
    }
}
