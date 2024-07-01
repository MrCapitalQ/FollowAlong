using MrCapitalQ.FollowAlong.Core.Capture;
using System.Drawing;

namespace MrCapitalQ.FollowAlong.Infrastructure.Capture;

public class GraphicsCreator : IGraphicsCreator<Image>
{
    public IGraphics CreateFrom(Image image) => new GraphicsAdapter(Graphics.FromImage(image));
}
