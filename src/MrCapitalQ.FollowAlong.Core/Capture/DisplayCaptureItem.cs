using MrCapitalQ.FollowAlong.Core.Display;
using MrCapitalQ.FollowAlong.Core.Utils;
using System;
using System.Diagnostics.CodeAnalysis;
using Windows.Foundation;
using Windows.Graphics.Capture;

namespace MrCapitalQ.FollowAlong.Core.Capture;

[ExcludeFromCodeCoverage(Justification = "Wrapper classes that simply forwards info from native GraphicsCaptureItem.")]
public class DisplayCaptureItem : IDisplayCaptureItem
{
    public event EventHandler? Closed;

    public DisplayCaptureItem(DisplayItem displayItem)
    {
        var graphicsCaptureItem = GraphicsCaptureItem.TryCreateFromDisplayId(new(displayItem.DisplayId))
            ?? throw new InvalidOperationException($"Failed to create graphics capture item from display ID {displayItem.DisplayId}.");

        GraphicsCaptureItem = graphicsCaptureItem;
        GraphicsCaptureItem.Closed += (_, _) => OnClosed();
        OuterBounds = displayItem.OuterBounds.ToRect();
    }

    public GraphicsCaptureItem GraphicsCaptureItem { get; }
    public Rect OuterBounds { get; }
    public string DisplayName => GraphicsCaptureItem.DisplayName;

    private void OnClosed()
    {
        var raiseEvent = Closed;
        raiseEvent?.Invoke(this, new());
    }
}
