using MrCapitalQ.FollowAlong.Core.Display;
using System.Drawing;

namespace MrCapitalQ.FollowAlong.Infrastructure.Capture;

public interface IScreenshotService
{
    Task<Stream> GetDisplayImageAsync(DisplayItem displayItem, Size size);
}