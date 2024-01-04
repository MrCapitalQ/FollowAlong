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
        private ObservableCollection<MonitorInfo> _monitors = new();

        [ObservableProperty]
        private MonitorInfo? _selectedMonitor;

        public MainViewModel(MonitorService monitorService,
            HotKeysService hotKeysService,
            BitmapCaptureService captureService)
        {
            hotKeysService.HotKeyInvoked += HotKeysService_HotKeyInvoked;

            Monitors = new(monitorService.GetAll());
            SelectedMonitor = Monitors.FirstOrDefault(x => x.IsPrimary);
            _captureService = captureService;
        }

        private void HotKeysService_HotKeyInvoked(object? sender, HotKeyInvokedEventArgs e)
        {
            if (e.HotKeyType == HotKeyType.StartStop)
            {
                if (!_captureService.IsStarted && SelectedMonitor is not null)
                {
                    var captureItem = SelectedMonitor.CreateCaptureItem();
                    _captureService.StartCapture(captureItem);
                }
                else if (_captureService.IsStarted)
                    _captureService.StopCapture();
            }
            else if (e.HotKeyType == HotKeyType.ZoomIn)
            { }
            else if (e.HotKeyType == HotKeyType.ZoomOut)
            { }
        }
    }
}
