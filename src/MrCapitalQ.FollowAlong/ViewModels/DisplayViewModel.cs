using Microsoft.UI.Xaml.Media.Imaging;
using MrCapitalQ.FollowAlong.Core.Display;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace MrCapitalQ.FollowAlong.ViewModels
{
    internal class DisplayViewModel
    {
        public DisplayViewModel(DisplayItem displayItem)
        {
            DisplayItem = displayItem;
            BitmapImage = new();

            _ = LoadThumbnailAsync();
        }

        public DisplayItem DisplayItem { get; }
        public BitmapImage BitmapImage { get; }
        public double AspectRatio => (double)DisplayItem.OuterBounds.Width / DisplayItem.OuterBounds.Height;

        public async Task LoadThumbnailAsync()
        {
            using var memoryStream = await Task.Run(() =>
            {
                using var bitmap = new Bitmap(DisplayItem.OuterBounds.Width, DisplayItem.OuterBounds.Height);
                using var graphics = Graphics.FromImage(bitmap);
                graphics.CopyFromScreen(DisplayItem.OuterBounds.X, DisplayItem.OuterBounds.Y, 0, 0, bitmap.Size);

                var memoryStream = new MemoryStream();
                bitmap.Save(memoryStream, ImageFormat.Png);
                memoryStream.Position = 0;
                return memoryStream;
            });
            await BitmapImage.SetSourceAsync(memoryStream.AsRandomAccessStream());
        }
    }
}
