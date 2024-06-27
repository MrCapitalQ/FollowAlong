using MrCapitalQ.FollowAlong.Core.Display;

namespace MrCapitalQ.FollowAlong.Core.Capture;

public interface IDisplayCaptureItemCreator
{
    IDisplayCaptureItem Create(DisplayItem displayItem);
}
