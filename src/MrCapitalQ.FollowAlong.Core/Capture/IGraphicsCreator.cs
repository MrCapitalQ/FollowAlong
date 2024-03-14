using System.Drawing;

namespace MrCapitalQ.FollowAlong.Core.Capture
{
    public interface IGraphicsCreator
    {
        IGraphics FromImage(Image image);
    }
}
