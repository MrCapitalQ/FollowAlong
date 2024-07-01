using MrCapitalQ.FollowAlong.Core.Capture;
using MrCapitalQ.FollowAlong.Core.Display;
using System.Drawing;
using System.Drawing.Imaging;

namespace MrCapitalQ.FollowAlong.Infrastructure.Capture;

public class ScreenshotService(IGraphicsCreator<Image> graphicsCreator) : IScreenshotService
{
    private readonly IGraphicsCreator<Image> _graphicsCreator = graphicsCreator;

    public async Task<Stream> GetDisplayImageAsync(DisplayItem displayItem, Size size)
        => await Task.Run(() =>
        {
            using var bitmap = new Bitmap(displayItem.OuterBounds.Width, displayItem.OuterBounds.Height);
            using var graphics = _graphicsCreator.CreateFrom(bitmap);
            graphics.CopyFromScreen(displayItem.OuterBounds.X, displayItem.OuterBounds.Y, 0, 0, bitmap.Size);

            using var resizedBitmap = new Bitmap(bitmap, size);
            var memoryStream = new MemoryStream();
            resizedBitmap.Save(memoryStream, ImageFormat.Jpeg);
            memoryStream.Position = 0;
            return memoryStream;
        });
}
