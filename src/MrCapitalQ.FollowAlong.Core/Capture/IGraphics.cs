using System.Drawing;

namespace MrCapitalQ.FollowAlong.Core.Capture;

public interface IGraphics : IDisposable
{
    void CopyFromScreen(int sourceX, int sourceY, int destinationX, int destinationY, Size blockRegionSize);
}
