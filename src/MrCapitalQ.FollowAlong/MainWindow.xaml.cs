using Microsoft.UI.Xaml;
using MrCapitalQ.FollowAlong.Core.Capture;
using MrCapitalQ.FollowAlong.Core.Monitors;
using MrCapitalQ.FollowAlong.Core.Tracking;
using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace MrCapitalQ.FollowAlong
{
    public sealed partial class MainWindow : Window
    {
        private readonly MonitorService _monitorService;
        private readonly BitmapCaptureService _captureService;
        private readonly TrackingTransformService _trackingTransformService;

        public MainWindow(MonitorService monitorService,
            BitmapCaptureService captureService,
            TrackingTransformService trackingTransformService)
        {
            InitializeComponent();
            _monitorService = monitorService;
            _captureService = captureService;
            _trackingTransformService = trackingTransformService;
        }

        private void CaptureButton_Click(object sender, RoutedEventArgs e)
        {
            var monitor = _monitorService.GetAll().Where(x => x.IsPrimary).First();
            var captureItem = monitor.CreateCaptureItem();
            _captureService.StartCapture(captureItem, Preview);
            _trackingTransformService.StartTrackingTransforms(Preview);

            //ExcludeWindowFromCapture();
        }

        [DllImport("user32.dll")]
        private static extern uint SetWindowDisplayAffinity(IntPtr hwnd, uint dwAffinity);

        private void ExcludeWindowFromCapture()
        {
            const uint WDA_EXCLUDEFROMCAPTURE = 0x00000011;

            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            _ = SetWindowDisplayAffinity(hWnd, WDA_EXCLUDEFROMCAPTURE);
        }
    }
}
