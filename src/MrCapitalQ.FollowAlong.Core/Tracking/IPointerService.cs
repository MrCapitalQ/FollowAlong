using Windows.Foundation;

namespace MrCapitalQ.FollowAlong.Core.Tracking
{
    public interface IPointerService
    {
        Point? GetCurrentPosition();
    }
}