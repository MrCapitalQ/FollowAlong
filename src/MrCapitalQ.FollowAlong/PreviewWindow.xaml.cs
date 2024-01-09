using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml;
using MrCapitalQ.FollowAlong.Core.Capture;
using MrCapitalQ.FollowAlong.Core.Monitors;
using MrCapitalQ.FollowAlong.Core.Tracking;
using System;
using System.Runtime.InteropServices;
using Windows.Graphics;
using WinUIEx;

namespace MrCapitalQ.FollowAlong
{
    public sealed partial class PreviewWindow : Window
    {
        private const int BottomPadding = 48;
        private readonly static SizeInt32 s_windowSize = new(480, 320);
        private readonly TrackingTransformService _trackingTransformService;

        public PreviewWindow(BitmapCaptureService captureService,
            TrackingTransformService trackingTransformService)
        {
            InitializeComponent();

            captureService.RegisterFrameHandler(Preview);

            _trackingTransformService = trackingTransformService;
            _trackingTransformService.StartTrackingTransforms(Preview);
            WeakReferenceMessenger.Default.Register<ZoomChanged>(this,
                (r, m) => _trackingTransformService.Zoom = m.Zoom);

            ExcludeWindowFromCapture();
            this.SetIsShownInSwitchers(false);
            this.SetIsResizable(false);
            this.SetIsMinimizable(false);
            this.SetIsMaximizable(false);
            this.SetIsAlwaysOnTop(true);

            ExtendsContentIntoTitleBar = true;
            AppWindow.Resize(s_windowSize);
            RepositionToPreviewPosition();

            Root.Loaded += Root_Loaded;
            Activated += PreviewWindow_Activated;
        }

        private void RepositionToPreviewPosition()
        {
            var appMonitor = this.GetWindowMonitorSize();
            if (appMonitor is not null)
                AppWindow.Move(new PointInt32(0,
                    (int)(appMonitor.ScreenSize.Y - s_windowSize.Height - (BottomPadding * Root.XamlRoot?.RasterizationScale ?? 1))));
        }

        private void Root_Loaded(object sender, RoutedEventArgs e) => RepositionToPreviewPosition();

        private void PreviewWindow_Activated(object sender, WindowActivatedEventArgs args)
            => _trackingTransformService.UpdateLayout();

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
