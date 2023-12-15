using Microsoft.UI.Composition;
using Windows.Foundation;

namespace MrCapitalQ.FollowAlong.Core.Tracking
{
    public interface ITrackingTransformTarget
    {
        CompositionSurfaceBrush? Brush { get; }
        Size ContentSize { get; }
        Size ViewportSize { get; }
        double RenderScale { get; }
    }
}
