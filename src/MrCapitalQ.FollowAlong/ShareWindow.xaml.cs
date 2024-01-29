using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using MrCapitalQ.FollowAlong.Core.Capture;
using MrCapitalQ.FollowAlong.Core.Monitors;
using MrCapitalQ.FollowAlong.Core.Tracking;
using MrCapitalQ.FollowAlong.Messages;
using Windows.Graphics;

namespace MrCapitalQ.FollowAlong
{
    public sealed partial class ShareWindow : Window
    {
        private const double ViewportAspectRatio = 16 / 9d;

        private readonly BitmapCaptureService _captureService;
        private readonly TrackingTransformService _trackingTransformService;

        public ShareWindow(BitmapCaptureService captureService,
            TrackingTransformService trackingTransformService)
        {
            InitializeComponent();

            _captureService = captureService;
            _captureService.RegisterFrameHandler(Preview);

            _trackingTransformService = trackingTransformService;
            _trackingTransformService.StartTrackingTransforms(Preview);
            WeakReferenceMessenger.Default.Register<ZoomChanged>(this,
                (r, m) => _trackingTransformService.Zoom = m.Zoom);

            if (AppWindow.Presenter is OverlappedPresenter presenter)
            {
                presenter.IsResizable = false;
                presenter.IsMinimizable = false;
                presenter.IsMaximizable = false;
                presenter.SetBorderAndTitleBar(false, false);
            }

            ExtendsContentIntoTitleBar = true;
            RepositionToSharingPosition();

            Activated += ShareWindow_Activated;
            Closed += ShareWindow_Closed;
        }

        public void SetScreenSize(SizeInt32 size)
        {
            var viewportSize = (size.Width / size.Height) > ViewportAspectRatio
                            ? new SizeInt32((int)(size.Height * ViewportAspectRatio), size.Height)
                            : new SizeInt32(size.Width, (int)(size.Width / ViewportAspectRatio));
            AppWindow.Resize(viewportSize);
        }

        private void RepositionToSharingPosition()
        {
            var appMonitor = this.GetCurrentMonitorInfo();
            if (appMonitor is not null)
                AppWindow.Move(new PointInt32((int)appMonitor.ScreenSize.X - 1, (int)appMonitor.ScreenSize.Y - 1));
        }

        private void ShareWindow_Activated(object sender, WindowActivatedEventArgs args)
            => _trackingTransformService.UpdateLayout();

        private void ShareWindow_Closed(object sender, WindowEventArgs args)
        {
            Activated -= ShareWindow_Activated;
            Closed -= ShareWindow_Closed;
            _captureService.UnregisterFrameHandler(Preview);
            _trackingTransformService.StopTrackingTransforms();
            WeakReferenceMessenger.Default.UnregisterAll(this);
        }
    }
}
