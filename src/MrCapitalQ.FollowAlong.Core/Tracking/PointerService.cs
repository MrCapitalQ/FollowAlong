using System.Diagnostics.CodeAnalysis;
using Windows.Foundation;

namespace MrCapitalQ.FollowAlong.Core.Tracking
{
    [ExcludeFromCodeCoverage(Justification = "Forwards calls to a native interop call that can't be mocked.")]
    public class PointerService : IPointerService
    {
        public Point? GetCurrentPosition() => PointerInterops.GetCursorPosition();
    }
}
