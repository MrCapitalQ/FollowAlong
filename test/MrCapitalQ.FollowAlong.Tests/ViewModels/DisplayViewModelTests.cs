using MrCapitalQ.FollowAlong.Core.Capture;
using MrCapitalQ.FollowAlong.Core.Display;
using MrCapitalQ.FollowAlong.ViewModels;
using NSubstitute;

namespace MrCapitalQ.FollowAlong.Tests.ViewModels
{
    public class DisplayViewModelTests
    {
        [Fact]
        public void Ctor_SetsProperties()
        {
            var displayItem = new DisplayItem(true, new(0, 0, 10, 5), new(), 1); ;
            var stream = new MemoryStream([0x01, 0x02, 0x03]);
            var screenshotService = Substitute.For<IScreenshotService>();
            screenshotService.GetDisplayImageAsync(displayItem).Returns(stream);

            var vm = new DisplayViewModel(displayItem, screenshotService);

            Assert.Equal(stream, vm.Thumbnail);
            Assert.Equal(displayItem.OuterBounds.Width / displayItem.OuterBounds.Height, vm.AspectRatio);
            screenshotService.Received(1).GetDisplayImageAsync(displayItem);
        }

        [Fact]
        public async Task LoadThumbnailAsync_DisposesExistingThumbnailStream()
        {
            var displayItem = new DisplayItem(true, new(), new(), 1); ;
            var stream1 = Substitute.For<Stream>();
            var stream2 = Substitute.For<Stream>();
            var screenshotService = Substitute.For<IScreenshotService>();
            screenshotService.GetDisplayImageAsync(displayItem).Returns(stream1, stream2);
            var vm = new DisplayViewModel(displayItem, screenshotService);

            await vm.LoadThumbnailAsync();

            Assert.Equal(stream2, vm.Thumbnail);
            await screenshotService.Received(2).GetDisplayImageAsync(displayItem);
            stream1.Received(1).Dispose();
        }
    }
}