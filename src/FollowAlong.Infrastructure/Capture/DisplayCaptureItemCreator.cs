using MrCapitalQ.FollowAlong.Core.Capture;
using MrCapitalQ.FollowAlong.Core.Display;
using System.Diagnostics.CodeAnalysis;
using Windows.Graphics.Capture;

namespace MrCapitalQ.FollowAlong.Infrastructure.Capture;

[ExcludeFromCodeCoverage(Justification = "Creates a native GraphicsCaptureItem object that can't be created during test execution.")]
public class DisplayCaptureItemCreator : IDisplayCaptureItemCreator
{
    public IDisplayCaptureItem Create(DisplayItem displayItem)
    {
        var graphicsCaptureItem = GraphicsCaptureItem.TryCreateFromDisplayId(new(displayItem.DisplayId))
            ?? throw new InvalidOperationException($"Failed to create graphics capture item from display ID {displayItem.DisplayId}.");

        return new DisplayCaptureItem(graphicsCaptureItem, displayItem.OuterBounds);
    }
}
