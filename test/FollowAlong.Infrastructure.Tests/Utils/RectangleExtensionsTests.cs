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

    [InlineData(4, 4, 10, 10, 0, 0, false)]   // Point to left and above of rectangle
    [InlineData(4, 4, 10, 10, 4, 0, false)]   // Point above left edge of rectangle
    [InlineData(4, 4, 10, 10, 9, 0, false)]   // Point above center of rectangle
    [InlineData(4, 4, 10, 10, 14, 0, false)]  // Point above right edge of rectangle
    [InlineData(4, 4, 10, 10, 18, 0, false)]  // Point to right and above of rectangle
    [InlineData(4, 4, 10, 10, 18, 4, false)]  // Point to right of top edge of rectangle
    [InlineData(4, 4, 10, 10, 18, 9, false)]  // Point to right of center rectangle
    [InlineData(4, 4, 10, 10, 18, 14, false)] // Point to right of bottom edge of rectangle
    [InlineData(4, 4, 10, 10, 18, 18, false)] // Point to right and below of rectangle
    [InlineData(4, 4, 10, 10, 14, 18, false)] // Point below right edge of rectangle
    [InlineData(4, 4, 10, 10, 9, 18, false)]  // Point below center rectangle
    [InlineData(4, 4, 10, 10, 4, 18, false)]  // Point below left edge of rectangle
    [InlineData(4, 4, 10, 10, 0, 18, false)]  // Point to left and below rectangle
    [InlineData(4, 4, 10, 10, 0, 14, false)]  // Point to left of bottom edge of rectangle
    [InlineData(4, 4, 10, 10, 0, 9, false)]   // Point to left of center of rectangle
    [InlineData(4, 4, 10, 10, 0, 4, false)]   // Point to left of top edge of rectangle
    [InlineData(4, 4, 10, 10, 9, 9, true)]    // Point inside of rectangle
    [InlineData(4, 4, 10, 10, 4, 4, true)]    // Point on top-left corner of rectangle
    [InlineData(4, 4, 10, 10, 14, 4, true)]   // Point on top-right corner of rectangle
    [InlineData(4, 4, 10, 10, 4, 14, true)]   // Point on bottom-left corner of rectangle
    [InlineData(4, 4, 10, 10, 14, 14, true)]  // Point on bottom-right corner of rectangle
    [InlineData(4, 4, 10, 10, 9, 4, true)]    // Point on center top edge of rectangle
    [InlineData(4, 4, 10, 10, 14, 9, true)]   // Point on center right edge of rectangle
    [InlineData(4, 4, 10, 10, 9, 14, true)]   // Point on center bottom edge of rectangle
    [InlineData(4, 4, 10, 10, 4, 9, true)]    // Point on center left edge of rectangle
    [Theory]
    public void Contains_ReturnsWhetherPointIsInsideRect(int rectX,
        int rectY,
        int rectWidth,
        int rectHeight,
        int pointX,
        int pointY,
        bool expected)
    {
        var actual = new RectInt32(rectX, rectY, rectWidth, rectHeight).Contains(new Point(pointX, pointY));

        Assert.Equal(expected, actual);
    }
}
