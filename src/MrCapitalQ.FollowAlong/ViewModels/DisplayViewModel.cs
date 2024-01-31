using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace MrCapitalQ.FollowAlong.ViewModels
{
    internal class DisplayViewModel
    {
        public DisplayViewModel(DisplayArea displayArea)
        {
            DisplayArea = displayArea;
            BitmapImage = new();

            _ = LoadThumbnailAsync();
        }

        public DisplayArea DisplayArea { get; }
        public BitmapImage BitmapImage { get; }
        public double AspectRatio => (double)DisplayArea.OuterBounds.Width / DisplayArea.OuterBounds.Height;

        public async Task LoadThumbnailAsync()
        {
            using var memoryStream = await Task.Run(() =>
            {
                using var bitmap = new Bitmap(DisplayArea.OuterBounds.Width, DisplayArea.OuterBounds.Height);
                using var graphics = Graphics.FromImage(bitmap);
                graphics.CopyFromScreen(DisplayArea.OuterBounds.X, DisplayArea.OuterBounds.Y, 0, 0, bitmap.Size);

                var memoryStream = new MemoryStream();
                bitmap.Save(memoryStream, ImageFormat.Png);
                memoryStream.Position = 0;
                return memoryStream;
            });
            await BitmapImage.SetSourceAsync(memoryStream.AsRandomAccessStream());
        }
    }
}
