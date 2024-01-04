using Microsoft.UI.Windowing;
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
            _captureService.RegisterFrameHandler(Preview);
            _captureService.Started += CaptureService_Started;
            _captureService.Stopped += CaptureService_Stopped;

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

            var displaySize = GetCurrentDisplaySize();
            if (displaySize.HasValue)
                AppWindow.Move(new PointInt32(displaySize.Value.Width - 1, displaySize.Value.Height - 1));

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

        private SizeInt32? GetCurrentDisplaySize()
        {
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var hmon = MonitorFromWindow(hwnd, MonitorFlag.MONITOR_DEFAULTTONEAREST);
            var currentDisplay = _monitorService.GetAll().FirstOrDefault(x => x.Hmon == hmon);
            if (currentDisplay is null)
                return null;

            return new SizeInt32((int)currentDisplay.ScreenSize.X, (int)currentDisplay.ScreenSize.Y);
        }


        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromWindow(IntPtr hwnd, MonitorFlag flag);

        /// <summary>
        /// Determines the function's return value if the window does not intersect any display monitor.
        /// </summary>
        private enum MonitorFlag : uint
        {
            /// <summary>Returns NULL.</summary>
            MONITOR_DEFAULTTONULL = 0,
            /// <summary>Returns a handle to the primary display monitor.</summary>
            MONITOR_DEFAULTTOPRIMARY = 1,
            /// <summary>Returns a handle to the display monitor that is nearest to the window.</summary>
            MONITOR_DEFAULTTONEAREST = 2
        }
    }
}
