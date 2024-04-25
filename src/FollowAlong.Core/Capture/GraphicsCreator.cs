using System.Drawing;

namespace MrCapitalQ.FollowAlong.Core.Capture;

public class GraphicsCreator : IGraphicsCreator
{
    public IGraphics FromImage(Image image) => new GraphicsAdapter(Graphics.FromImage(image));
}
