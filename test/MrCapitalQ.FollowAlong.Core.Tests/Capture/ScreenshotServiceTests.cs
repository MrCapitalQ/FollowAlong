using MrCapitalQ.FollowAlong.Core.Capture;
using MrCapitalQ.FollowAlong.Core.Display;
using NSubstitute;
using System.Drawing;

namespace MrCapitalQ.FollowAlong.Core.Tests.Capture
{
    public class ScreenshotServiceTests
    {
        private readonly IGraphicsCreator _graphicsCreator;

        private readonly ScreenshotService _screenshotService;

        public ScreenshotServiceTests()
        {
            _graphicsCreator = Substitute.For<IGraphicsCreator>();
            _screenshotService = new(_graphicsCreator);
        }

        [Fact]
        public async Task GetDisplayImageAsync_ReturnsStreamWithDataAndAtStartPosition()
        {
            var displayItem = new DisplayItem(true, new(10, 10, 10, 10), new(), 1);
            _graphicsCreator.FromImage(Arg.Any<Image>()).Returns(x => new TestGraphics((Image)x[0]));

            var stream = await _screenshotService.GetDisplayImageAsync(displayItem);

            Assert.True(stream.Length > 0);
            Assert.Equal(0, stream.Position);
        }

        private class TestGraphics : IGraphics
        {
            private readonly Image _image;
            private readonly Graphics _graphics;

            public TestGraphics(Image image)
            {
                _image = image;
                _graphics = Graphics.FromImage(image);
            }

            public void CopyFromScreen(int sourceX, int sourceY, int destinationX, int destinationY, Size blockRegionSize)
            {
                _graphics.FillRegion(new SolidBrush(Color.Black),
                    new Region(new Rectangle(destinationX, destinationY, _image.Width, _image.Height)));
            }

            public void Dispose()
            { }
        }
    }
}
