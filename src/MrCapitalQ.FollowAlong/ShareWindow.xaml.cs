using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using MrCapitalQ.FollowAlong.Core.Capture;
using MrCapitalQ.FollowAlong.Core.Monitors;
using MrCapitalQ.FollowAlong.Core.Tracking;
using MrCapitalQ.FollowAlong.Messages;
using System.Linq;
using Windows.Graphics;

namespace MrCapitalQ.FollowAlong
{
    public sealed partial class ShareWindow : Window
    {
        private const double ViewportAspectRatio = 16 / 9d;

        private readonly BitmapCaptureService _captureService;
        private readonly TrackingTransformService _trackingTransformService;
        private readonly MonitorService _monitorService;

        public ShareWindow(BitmapCaptureService captureService,
            TrackingTransformService trackingTransformService,
            MonitorService monitorService)
        {
            InitializeComponent();

            _captureService = captureService;
            _captureService.RegisterFrameHandler(Preview);

            _trackingTransformService = trackingTransformService;
            _trackingTransformService.StartTrackingTransforms(Preview);

            _monitorService = monitorService;

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

            AppWindow.Changed += AppWindow_Changed;
            Activated += ShareWindow_Activated;
            Closed += ShareWindow_Closed;
        }

        private void AppWindow_Changed(AppWindow sender, AppWindowChangedEventArgs args)
        {
            if (!args.DidPositionChange)
                return;

            AppWindow.Changed -= AppWindow_Changed;
            RepositionToSharingPosition();
            AppWindow.Changed += AppWindow_Changed;
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
            var lowestMonitorArea = _monitorService.GetAll()
                .Select(x => x.MonitorArea)
                .Aggregate((x, y) =>
                {
                    return (x, y) switch
                    {
                        _ when x.Bottom < y.Bottom => y,
                        _ when x.Bottom == y.Bottom && x.Right < y.Right => y,
                        _ => x
                    };
                });

            // Move to the bottom, right most corner of the lowest-positioned monitor with 1px still in view so the
            // window content is still rendered.
            AppWindow.Move(new PointInt32((int)lowestMonitorArea.Right - 1, (int)lowestMonitorArea.Bottom - 1));
        }

        private void ShareWindow_Activated(object sender, WindowActivatedEventArgs args)
            => _trackingTransformService.UpdateLayout();

        private void ShareWindow_Closed(object sender, WindowEventArgs args)
        {
            AppWindow.Changed -= AppWindow_Changed;
            Activated -= ShareWindow_Activated;
            Closed -= ShareWindow_Closed;
            _captureService.UnregisterFrameHandler(Preview);
            _trackingTransformService.StopTrackingTransforms();
            WeakReferenceMessenger.Default.UnregisterAll(this);
        }
    }
}
