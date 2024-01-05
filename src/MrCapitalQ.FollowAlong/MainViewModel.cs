using CommunityToolkit.Mvvm.ComponentModel;
using MrCapitalQ.FollowAlong.Core.Capture;
using MrCapitalQ.FollowAlong.Core.HotKeys;
using MrCapitalQ.FollowAlong.Core.Monitors;
using System.Collections.ObjectModel;
using System.Linq;

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
        }

        public MonitorInfo MonitorInfo { get; }
        public double AspectRatio => MonitorInfo.ScreenSize.X / MonitorInfo.ScreenSize.Y;
    }
}
