using MrCapitalQ.FollowAlong.Core.Utils;
using Windows.Graphics;

namespace MrCapitalQ.FollowAlong.Core.Tests.Utils
{
    public class RectInt32ExtensionsTests
    {
        [Fact]
        public void ToRect_ConvertsRectInt32()
        {
            var rectInt32 = new RectInt32(100, 200, 300, 400);

            var actual = rectInt32.ToRect();

            Assert.Equal(rectInt32.X, actual.X);
            Assert.Equal(rectInt32.Y, actual.Y);
            Assert.Equal(rectInt32.Width, actual.Width);
            Assert.Equal(rectInt32.Height, actual.Height);
        }
    }
}
