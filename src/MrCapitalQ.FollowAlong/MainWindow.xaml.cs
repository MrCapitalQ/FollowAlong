using Microsoft.UI.Xaml;
using MrCapitalQ.FollowAlong.Core.Capture;
using MrCapitalQ.FollowAlong.Core.HotKeys;
using MrCapitalQ.FollowAlong.Core.Monitors;
using MrCapitalQ.FollowAlong.Core.Tracking;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using Windows.Graphics;
using WinUIEx;

namespace MrCapitalQ.FollowAlong
{
    public sealed partial class MainWindow : Window
    {
        private readonly MonitorService _monitorService;
        private readonly BitmapCaptureService _captureService;
        private readonly TrackingTransformService _trackingTransformService;
        private readonly HotKeysService _hotKeysService;

        public MainWindow(MonitorService monitorService,
            BitmapCaptureService captureService,
            TrackingTransformService trackingTransformService,
            HotKeysService hotKeysService)
        {
            InitializeComponent();
            _monitorService = monitorService;
            _captureService = captureService;

            _trackingTransformService = trackingTransformService;
            _trackingTransformService.Zoom = 1.5;

            _hotKeysService = hotKeysService;
            _hotKeysService.RegisterHotKeys(this);
            _hotKeysService.HotKeyInvoked += HotKeysService_HotKeyInvoked;

            ExtendsContentIntoTitleBar = true;
            AppWindow.Resize(new SizeInt32(640, 480));
            this.CenterOnScreen();
        }

        private void CaptureButton_Click(object sender, RoutedEventArgs e) => StartCapture();

        private void StartCapture()
        {
            var monitor = _monitorService.GetAll().Where(x => x.IsPrimary).First();
            var captureItem = monitor.CreateCaptureItem();
            _captureService.StartCapture(captureItem, Preview);
            _trackingTransformService.StartTrackingTransforms(Preview);

            CaptureButton.Visibility = Visibility.Collapsed;

            AppWindow.ResizeClient(new SizeInt32(1280, 720));
            AppWindow.Move(new PointInt32((int)monitor.ScreenSize.X - 1, (int)monitor.ScreenSize.Y - 1));
            this.SetIsResizable(false);
            this.SetIsMinimizable(false);
            this.SetIsMaximizable(false);

            //ExcludeWindowFromCapture();
            //this.CenterOnScreen();
        }

        private void StopCapture()
        {
            _captureService.StopCapture();
            _trackingTransformService.StopTrackingTransforms();
        }

        private void HotKeysService_HotKeyInvoked(object? sender, HotKeyInvokedEventArgs e)
        {
            if (e.HotKeyType == HotKeyType.StartStop)
            {
                if (_captureService.IsStarted)
                    StopCapture();
                else
                    StartCapture();
            }
            else if (e.HotKeyType == HotKeyType.ZoomIn)
                _trackingTransformService.Zoom = Math.Min(_trackingTransformService.Zoom + 0.5, 3);
            else if (e.HotKeyType == HotKeyType.ZoomOut)
                _trackingTransformService.Zoom = Math.Max(_trackingTransformService.Zoom - 0.5, 1);
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
