using MrCapitalQ.FollowAlong.Core.Display;
using System.IO;
using System.Threading.Tasks;

namespace MrCapitalQ.FollowAlong.Core.Capture
{
    public interface IScreenshotService
    {
        Task<Stream> GetDisplayImageAsync(DisplayItem displayItem);
    }
}