using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml;
using MrCapitalQ.FollowAlong.Core.Capture;
using MrCapitalQ.FollowAlong.Core.Monitors;
using MrCapitalQ.FollowAlong.Core.Tracking;
using MrCapitalQ.FollowAlong.Core.Utils;
using Windows.Graphics;
using WinUIEx;

namespace MrCapitalQ.FollowAlong
{
    public sealed partial class PreviewWindow : Window
    {
        private const int BottomPadding = 48;
        private readonly static SizeInt32 s_windowSize = new(480, 270);
        private readonly BitmapCaptureService _captureService;
        private readonly TrackingTransformService _trackingTransformService;

        public PreviewWindow(BitmapCaptureService captureService,
            TrackingTransformService trackingTransformService)
        {
            InitializeComponent();

            _captureService = captureService;
            _captureService.RegisterFrameHandler(Preview);

            _trackingTransformService = trackingTransformService;
            _trackingTransformService.StartTrackingTransforms(Preview);
            WeakReferenceMessenger.Default.Register<ZoomChanged>(this,
                (r, m) => _trackingTransformService.Zoom = m.Zoom);

            this.ExcludeFromCapture();
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
            Closed += PreviewWindow_Closed;
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

        private void PreviewWindow_Closed(object sender, WindowEventArgs args)
        {
            Root.Loaded -= Root_Loaded;
            Activated -= PreviewWindow_Activated;
            Closed -= PreviewWindow_Closed;
            _captureService.UnregisterFrameHandler(Preview);
            _trackingTransformService.StopTrackingTransforms();
            WeakReferenceMessenger.Default.UnregisterAll(this);
        }
    }
}
