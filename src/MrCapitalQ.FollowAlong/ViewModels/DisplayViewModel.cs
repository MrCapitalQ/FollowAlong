using CommunityToolkit.Mvvm.ComponentModel;
using MrCapitalQ.FollowAlong.Core.Capture;
using MrCapitalQ.FollowAlong.Core.Display;
using System.IO;
using System.Threading.Tasks;

namespace MrCapitalQ.FollowAlong.ViewModels
{
    internal class DisplayViewModel : ObservableObject
    {
        private readonly IScreenshotService _screenshotService;

        public Stream? _thumbnail;

        public DisplayViewModel(DisplayItem displayItem, IScreenshotService screenshotService)
        {
            DisplayItem = displayItem;
            _screenshotService = screenshotService;

            _ = LoadThumbnailAsync();
        }

        public DisplayItem DisplayItem { get; }

        public Stream? Thumbnail
        {
            get => _thumbnail;
            private set
            {
                _thumbnail?.Dispose();
                _thumbnail = value;
                OnPropertyChanged(nameof(Thumbnail));
            }
        }

        public double AspectRatio => (double)DisplayItem.OuterBounds.Width / DisplayItem.OuterBounds.Height;

        public async Task LoadThumbnailAsync()
            => Thumbnail = await _screenshotService.GetDisplayImageAsync(DisplayItem);
    }
}
