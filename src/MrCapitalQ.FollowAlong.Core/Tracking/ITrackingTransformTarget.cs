using Microsoft.UI.Composition;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Windows.Foundation;

namespace MrCapitalQ.FollowAlong.Core.Tracking
{
    public interface ITrackingTransformTarget
    {
        event SizeChangedEventHandler SizeChanged;
        CompositionSurfaceBrush? Brush { get; }
        Rect ContentArea { get; }
        Size ViewportSize { get; }
        DispatcherQueue DispatcherQueue { get; }
    }
}
