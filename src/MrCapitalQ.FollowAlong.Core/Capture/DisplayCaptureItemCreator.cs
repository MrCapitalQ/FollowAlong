using MrCapitalQ.FollowAlong.Core.Display;

namespace MrCapitalQ.FollowAlong.Core.Capture;

public class DisplayCaptureItemCreator : IDisplayCaptureItemCreator
{
    public IDisplayCaptureItem Create(DisplayItem displayItem) => new DisplayCaptureItem(displayItem);
}
