using MrCapitalQ.FollowAlong.Infrastructure.Display;

namespace MrCapitalQ.FollowAlong.Infrastructure.Capture;

public interface IDisplayCaptureItemCreator
{
    IDisplayCaptureItem Create(DisplayItem displayItem);
}
