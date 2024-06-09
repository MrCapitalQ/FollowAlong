using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using Windows.Graphics.Capture;

namespace MrCapitalQ.FollowAlong.Infrastructure.Capture;

[ExcludeFromCodeCoverage(Justification = "Wrapper classes that simply forwards info from native GraphicsCaptureItem.")]
public class DisplayCaptureItem : IDisplayCaptureItem
{
    public event EventHandler? Closed;

    internal DisplayCaptureItem(GraphicsCaptureItem graphicsCaptureItem, Rectangle outerBounds)
    {
        GraphicsCaptureItem = graphicsCaptureItem;
        GraphicsCaptureItem.Closed += (_, _) => OnClosed();
        OuterBounds = outerBounds;
    }

    public GraphicsCaptureItem GraphicsCaptureItem { get; }
    public Rectangle OuterBounds { get; }
    public string DisplayName => GraphicsCaptureItem.DisplayName;

    private void OnClosed()
    {
        var raiseEvent = Closed;
        raiseEvent?.Invoke(this, new());
    }
}
