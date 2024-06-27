using MrCapitalQ.FollowAlong.Core.Display;
using MrCapitalQ.FollowAlong.Infrastructure.Capture;
using System.Drawing;

namespace MrCapitalQ.FollowAlong.Infrastructure.Tests.Capture;

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

        var stream = await _screenshotService.GetDisplayImageAsync(displayItem, new Size(10, 10));

        Assert.True(stream.Length > 0);
        Assert.Equal(0, stream.Position);
    }

    private class TestGraphics(Image image) : IGraphics
    {
        private readonly Image _image = image;
        private readonly Graphics _graphics = Graphics.FromImage(image);

        public void CopyFromScreen(int sourceX, int sourceY, int destinationX, int destinationY, Size blockRegionSize)
        {
            _graphics.FillRegion(new SolidBrush(Color.Black),
                new Region(new Rectangle(destinationX, destinationY, _image.Width, _image.Height)));
        }

        public void Dispose()
        { }
    }
}
