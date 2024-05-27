using System.Drawing;

namespace MrCapitalQ.FollowAlong.Infrastructure.Capture;

public class GraphicsCreator : IGraphicsCreator
{
    public IGraphics FromImage(Image image) => new GraphicsAdapter(Graphics.FromImage(image));
}
