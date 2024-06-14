using System.Drawing;

namespace MrCapitalQ.FollowAlong.Core.Tracking;

public interface IPointerService
{
    Point? GetCurrentPosition();
}