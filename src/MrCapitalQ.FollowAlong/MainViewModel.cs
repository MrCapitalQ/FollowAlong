using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Media.Imaging;
using MrCapitalQ.FollowAlong.Core.Capture;
using MrCapitalQ.FollowAlong.Core.HotKeys;
using MrCapitalQ.FollowAlong.Core.Monitors;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MrCapitalQ.FollowAlong
{
    internal partial class MainViewModel : ObservableObject
    {
        private readonly BitmapCaptureService _captureService;

        [ObservableProperty]
        private ObservableCollection<MonitorViewModel> _monitors = new();

        [ObservableProperty]
        private MonitorViewModel? _selectedMonitor;

        public MainViewModel(MonitorService monitorService,
            HotKeysService hotKeysService,
            BitmapCaptureService captureService)
        {
            hotKeysService.HotKeyInvoked += HotKeysService_HotKeyInvoked;

            Monitors = new(monitorService.GetAll().Select(x => new MonitorViewModel(x)));
            SelectedMonitor = Monitors.FirstOrDefault(x => x.MonitorInfo.IsPrimary);
            _captureService = captureService;
        }

        private void HotKeysService_HotKeyInvoked(object? sender, HotKeyInvokedEventArgs e)
        {
            if (e.HotKeyType != HotKeyType.StartStop)
                return;

            if (!_captureService.IsStarted && SelectedMonitor is not null)
            {
                var captureItem = SelectedMonitor.MonitorInfo.CreateCaptureItem();
                _captureService.StartCapture(captureItem);
            }
            else if (_captureService.IsStarted)
                _captureService.StopCapture();
        }
    }

    internal class MonitorViewModel
    {
        public MonitorViewModel(MonitorInfo monitorInfo)
        {
            MonitorInfo = monitorInfo;
            BitmapImage = new();

            _ = LoadThumbnailAsync();
        }

        public MonitorInfo MonitorInfo { get; }
        public BitmapImage BitmapImage { get; }
        public double AspectRatio => MonitorInfo.ScreenSize.X / MonitorInfo.ScreenSize.Y;

        private async Task LoadThumbnailAsync()
        {
            using var bitmap = new Bitmap((int)MonitorInfo.MonitorArea.Width, (int)MonitorInfo.MonitorArea.Height);
            using var graphics = Graphics.FromImage(bitmap);
            graphics.CopyFromScreen(0, 0, 0, 0, bitmap.Size);

            using var memoryStream = new MemoryStream();
            bitmap.Save(memoryStream, ImageFormat.Png);
            memoryStream.Position = 0;

            await BitmapImage.SetSourceAsync(memoryStream.AsRandomAccessStream());
        }
    }
}
