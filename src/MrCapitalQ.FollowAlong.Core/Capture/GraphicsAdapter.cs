using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace MrCapitalQ.FollowAlong.Core.Capture
{
    [ExcludeFromCodeCoverage(Justification = "Adapter class for native graphics classes that can't be unit tested.")]
    public sealed class GraphicsAdapter : IGraphics
    {
        private readonly Graphics _graphics;

        public GraphicsAdapter(Graphics graphics) => _graphics = graphics;

        public void CopyFromScreen(int sourceX, int sourceY, int destinationX, int destinationY, Size blockRegionSize)
            => _graphics.CopyFromScreen(sourceX, sourceY, destinationX, destinationY, blockRegionSize);

        public void Dispose() => _graphics.Dispose();
    }
}
