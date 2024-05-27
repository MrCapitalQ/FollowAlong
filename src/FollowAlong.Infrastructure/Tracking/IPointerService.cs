using Windows.Foundation;

namespace MrCapitalQ.FollowAlong.Infrastructure.Tracking;

public interface IPointerService
{
    Point? GetCurrentPosition();
}