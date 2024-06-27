using MrCapitalQ.FollowAlong.Infrastructure.Capture;
using System.Drawing;

namespace MrCapitalQ.FollowAlong.Infrastructure.Tests.Capture;

public class GraphicsCreatorTests
{
    [Fact]
    public void CreateFrom_CreatesGraphicsAdapter()
    {
        var graphics = new GraphicsCreator().CreateFrom(new Bitmap(1, 1));

        Assert.NotNull(graphics);
        Assert.IsType<GraphicsAdapter>(graphics);
    }
}
