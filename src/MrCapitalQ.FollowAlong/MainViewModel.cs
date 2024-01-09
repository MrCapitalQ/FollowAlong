using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
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
        private const double MaxZoom = 3;
        private const double MinZoom = 1;
        private const double ZoomStepSize = 0.5;
        private readonly BitmapCaptureService _captureService;
        private double _zoom = 1.5;

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
            if (e.HotKeyType is HotKeyType.StartStop)
            {

                if (!_captureService.IsStarted && SelectedMonitor is not null)
                {
                    var captureItem = SelectedMonitor.MonitorInfo.CreateCaptureItem();
                    _captureService.StartCapture(captureItem);
                    WeakReferenceMessenger.Default.Send(new ZoomChanged(_zoom));
                }
                else if (_captureService.IsStarted)
                    _captureService.StopCapture();
            }
            else if (e.HotKeyType == HotKeyType.ZoomIn)
                WeakReferenceMessenger.Default.Send(new ZoomChanged(_zoom = Math.Min(_zoom + ZoomStepSize, MaxZoom)));
            else if (e.HotKeyType == HotKeyType.ZoomOut)
                WeakReferenceMessenger.Default.Send(new ZoomChanged(_zoom = Math.Max(_zoom - ZoomStepSize, MinZoom)));
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
            using var memoryStream = await Task.Run(() =>
            {
                using var bitmap = new Bitmap((int)MonitorInfo.MonitorArea.Width, (int)MonitorInfo.MonitorArea.Height);
                using var graphics = Graphics.FromImage(bitmap);
                graphics.CopyFromScreen((int)MonitorInfo.MonitorArea.Left, (int)MonitorInfo.MonitorArea.Top, 0, 0, bitmap.Size);

                var memoryStream = new MemoryStream();
                bitmap.Save(memoryStream, ImageFormat.Png);
                memoryStream.Position = 0;
                return memoryStream;
            });
            await BitmapImage.SetSourceAsync(memoryStream.AsRandomAccessStream());
        }
    }
}
