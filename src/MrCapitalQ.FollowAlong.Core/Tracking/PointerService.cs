using System.Diagnostics.CodeAnalysis;
using Windows.Foundation;

namespace MrCapitalQ.FollowAlong.Core.Tracking
{
    [ExcludeFromCodeCoverage]
    public class PointerService : IPointerService
    {
        public Point? GetCurrentPosition() => PointerInterops.GetCursorPosition();
    }
}
