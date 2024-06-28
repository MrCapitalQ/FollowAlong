using MrCapitalQ.FollowAlong.Infrastructure.Utils;
using System.Drawing;
using Windows.Graphics;

namespace MrCapitalQ.FollowAlong.Infrastructure.Tests.Utils;

public class RectangleExtensionsTests
{
    [Fact]
    public void ToRectangle_ConvertsRectInt32()
    {
        var rectInt32 = new RectInt32(100, 200, 300, 400);

        var actual = rectInt32.ToRectangle();

        Assert.Equal(rectInt32.X, actual.X);
        Assert.Equal(rectInt32.Y, actual.Y);
        Assert.Equal(rectInt32.Width, actual.Width);
        Assert.Equal(rectInt32.Height, actual.Height);
    }

    [Fact]
    public void ToSizeInt32_ConvertsRectangleToSizeInt32()
    {
        var rect = new Rectangle(100, 200, 300, 400);

        var actual = rect.ToSizeInt32();

        Assert.Equal(rect.Width, actual.Width);
        Assert.Equal(rect.Height, actual.Height);
    }
}
