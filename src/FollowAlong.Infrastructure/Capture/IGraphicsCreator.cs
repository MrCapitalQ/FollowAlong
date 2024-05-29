using System.Drawing;

namespace MrCapitalQ.FollowAlong.Infrastructure.Capture;

public interface IGraphicsCreator
{
    IGraphics FromImage(Image image);
}
