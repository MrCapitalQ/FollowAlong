using MrCapitalQ.FollowAlong.Core.Display;

namespace MrCapitalQ.FollowAlong.Infrastructure.Capture;

public interface IDisplayCaptureItemCreator
{
    IDisplayCaptureItem Create(DisplayItem displayItem);
}
