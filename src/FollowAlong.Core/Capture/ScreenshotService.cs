using MrCapitalQ.FollowAlong.Core.Display;
using System.Drawing;
using System.Drawing.Imaging;

namespace MrCapitalQ.FollowAlong.Core.Capture;

public class ScreenshotService : IScreenshotService
{
    private readonly IGraphicsCreator _graphicsCreator;

    public ScreenshotService(IGraphicsCreator graphicsCreator) => _graphicsCreator = graphicsCreator;

    public async Task<Stream> GetDisplayImageAsync(DisplayItem displayItem, Size size)
        => await Task.Run(() =>
        {
            using var bitmap = new Bitmap(displayItem.OuterBounds.Width, displayItem.OuterBounds.Height);
            using var graphics = _graphicsCreator.FromImage(bitmap);
            graphics.CopyFromScreen(displayItem.OuterBounds.X, displayItem.OuterBounds.Y, 0, 0, bitmap.Size);

            using var resizedBitmap = new Bitmap(bitmap, size);
            var memoryStream = new MemoryStream();
            resizedBitmap.Save(memoryStream, ImageFormat.Jpeg);
            memoryStream.Position = 0;
            return memoryStream;
        });
}
