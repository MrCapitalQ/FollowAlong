using Microsoft.UI.Xaml;
using MrCapitalQ.FollowAlong.Core.Capture;
using MrCapitalQ.FollowAlong.Core.HotKeys;
using MrCapitalQ.FollowAlong.Core.Monitors;
using MrCapitalQ.FollowAlong.Core.Tracking;
using System;
using System.Linq;
using Windows.Graphics;
using WinUIEx;

namespace MrCapitalQ.FollowAlong
{
    internal sealed partial class MainWindow : Window
    {
        private const double MaxZoom = 3;
        private const double MinZoom = 1;
        private const double ZoomStepSize = 0.5;
        private readonly static SizeInt32 s_defaultWindowSize = new(640, 480);
        private readonly static SizeInt32 s_viewportWindowSize = new(1280, 720);

        private readonly MonitorService _monitorService;
        private readonly BitmapCaptureService _captureService;
        private readonly TrackingTransformService _trackingTransformService;
        private readonly MainViewModel _viewModel;

        public MainWindow(MonitorService monitorService,
            BitmapCaptureService captureService,
            TrackingTransformService trackingTransformService,
            HotKeysService hotKeysService,
            MainViewModel viewModel)
        {
            InitializeComponent();
            _monitorService = monitorService;
            _captureService = captureService;

            _trackingTransformService = trackingTransformService;
            _trackingTransformService.Zoom = 1.5;

            _viewModel = viewModel;
            hotKeysService.RegisterHotKeys(this);
            hotKeysService.HotKeyInvoked += HotKeysService_HotKeyInvoked;

            ExtendsContentIntoTitleBar = true;
            AppWindow.Resize(s_defaultWindowSize);
            this.CenterOnScreen();
        }

        private void StartCapture()
        {
            var monitor = _monitorService.GetAll().Where(x => x.IsPrimary).First();
            var captureItem = monitor.CreateCaptureItem();
            _captureService.StartCapture(captureItem, Preview);
            _trackingTransformService.StartTrackingTransforms(Preview);

            MainContent.Visibility = Visibility.Collapsed;

            AppWindow.ResizeClient(s_viewportWindowSize);
            AppWindow.Move(new PointInt32((int)monitor.ScreenSize.X - 1, (int)monitor.ScreenSize.Y - 1));
            this.SetIsResizable(false);
            this.SetIsMinimizable(false);
            this.SetIsMaximizable(false);
        }

        private void StopCapture()
        {
            _captureService.StopCapture();
            _trackingTransformService.StopTrackingTransforms();

            MainContent.Visibility = Visibility.Visible;

            AppWindow.Resize(s_defaultWindowSize);
            this.CenterOnScreen();
            this.SetIsResizable(true);
            this.SetIsMinimizable(true);
            this.SetIsMaximizable(true);
            this.SetForegroundWindow();
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
                _trackingTransformService.Zoom = Math.Min(_trackingTransformService.Zoom + ZoomStepSize, MaxZoom);
            else if (e.HotKeyType == HotKeyType.ZoomOut)
                _trackingTransformService.Zoom = Math.Max(_trackingTransformService.Zoom - ZoomStepSize, MinZoom);
        }
    }
}
