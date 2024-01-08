using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using MrCapitalQ.FollowAlong.Core.Capture;
using MrCapitalQ.FollowAlong.Core.HotKeys;
using MrCapitalQ.FollowAlong.Core.Monitors;
using MrCapitalQ.FollowAlong.Core.Tracking;
using System;
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

        private readonly TrackingTransformService _trackingTransformService;
        private readonly MainViewModel _viewModel;

        public MainWindow(BitmapCaptureService captureService,
            TrackingTransformService trackingTransformService,
            HotKeysService hotKeysService,
            MainViewModel viewModel)
        {
            InitializeComponent();

            captureService.RegisterFrameHandler(Preview);
            captureService.Started += CaptureService_Started;
            captureService.Stopped += CaptureService_Stopped;

            _trackingTransformService = trackingTransformService;
            _trackingTransformService.Zoom = 1.5;
            _trackingTransformService.StartTrackingTransforms(Preview);

            _viewModel = viewModel;

            hotKeysService.RegisterHotKeys(this);
            hotKeysService.HotKeyInvoked += HotKeysService_HotKeyInvoked;

            ExtendsContentIntoTitleBar = true;
            AppWindow.Resize(s_defaultWindowSize);
            this.CenterOnScreen();
        }

        private void CaptureService_Started(object? sender, EventArgs e)
        {
            MainContent.Visibility = Visibility.Collapsed;

            AppWindow.ResizeClient(s_viewportWindowSize);

            var appMonitor = this.GetWindowMonitorSize();
            if (appMonitor is not null)
                AppWindow.Move(new PointInt32((int)appMonitor.ScreenSize.X - 1, (int)appMonitor.ScreenSize.Y - 1));

            this.SetIsResizable(false);
            this.SetIsMinimizable(false);
            this.SetIsMaximizable(false);
        }

        private void CaptureService_Stopped(object? sender, EventArgs e)
        {
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
            if (e.HotKeyType == HotKeyType.ZoomIn)
                _trackingTransformService.Zoom = Math.Min(_trackingTransformService.Zoom + ZoomStepSize, MaxZoom);
            else if (e.HotKeyType == HotKeyType.ZoomOut)
                _trackingTransformService.Zoom = Math.Max(_trackingTransformService.Zoom - ZoomStepSize, MinZoom);
        }
    }
}
