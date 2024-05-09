using CommunityToolkit.Mvvm.ComponentModel;
using MrCapitalQ.FollowAlong.Core.Capture;
using MrCapitalQ.FollowAlong.Core.Display;
using System.Drawing;

namespace MrCapitalQ.FollowAlong.ViewModels;

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
    public int Width => (int)(Height * (double)DisplayItem.OuterBounds.Width / DisplayItem.OuterBounds.Height);
    public int Height { get; } = 120;

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

    public async Task LoadThumbnailAsync()
        => Thumbnail = await _screenshotService.GetDisplayImageAsync(DisplayItem, new Size(Width, Height));
}
