using MrCapitalQ.FollowAlong.Core.Display;

namespace MrCapitalQ.FollowAlong.Core.Capture;

public interface IScreenshotService
{
    Task<Stream> GetDisplayImageAsync(DisplayItem displayItem);
}