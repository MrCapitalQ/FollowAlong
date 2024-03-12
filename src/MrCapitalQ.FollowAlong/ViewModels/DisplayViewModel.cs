using Microsoft.UI.Xaml.Media.Imaging;
using MrCapitalQ.FollowAlong.Core.Capture;
using MrCapitalQ.FollowAlong.Core.Display;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MrCapitalQ.FollowAlong.ViewModels
{
    internal class DisplayViewModel
    {
        private readonly IScreenshotService _screenshotService;

        public DisplayViewModel(DisplayItem displayItem, IScreenshotService screenshotService)
        {
            DisplayItem = displayItem;
            _screenshotService = screenshotService;
            BitmapImage = new();

            _ = LoadThumbnailAsync();
        }

        public DisplayItem DisplayItem { get; }
        public BitmapImage BitmapImage { get; }
        public double AspectRatio => (double)DisplayItem.OuterBounds.Width / DisplayItem.OuterBounds.Height;

        public async Task LoadThumbnailAsync()
        {
            using var memoryStream = await _screenshotService.GetDisplayImageAsync(DisplayItem);
            await BitmapImage.SetSourceAsync(memoryStream.AsRandomAccessStream());
        }
    }
}
