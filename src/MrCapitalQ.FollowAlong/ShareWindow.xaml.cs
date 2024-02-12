using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using MrCapitalQ.FollowAlong.Core.Capture;
using MrCapitalQ.FollowAlong.Core.Display;
using MrCapitalQ.FollowAlong.Core.Tracking;
using MrCapitalQ.FollowAlong.Core.Utils;
using MrCapitalQ.FollowAlong.Messages;
using System;
using System.Linq;
using Windows.Graphics;

namespace MrCapitalQ.FollowAlong
{
    public sealed partial class ShareWindow : WindowBase
    {
        private const double ViewportAspectRatio = 16 / 9d;

        private readonly BitmapCaptureService _captureService;
        private readonly TrackingTransformService _trackingTransformService;
        private readonly DisplayService _displayService;
        private readonly DisplayWatcher _displayWatcher;

        public ShareWindow(BitmapCaptureService captureService,
            TrackingTransformService trackingTransformService,
            DisplayService displayService,
            DisplayWatcher displayWatcher)
        {
            InitializeComponent();

            _captureService = captureService;
            _captureService.RegisterFrameHandler(Preview);

            _trackingTransformService = trackingTransformService;
            _trackingTransformService.StartTrackingTransforms(Preview);

            _displayService = displayService;

            _displayWatcher = displayWatcher;
            _displayWatcher.Register(this);
            _displayWatcher.DisplayChanged += DisplayWatcher_DisplayChanged;

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
            var lowestDisplayArea = _displayService.GetAll()
                .Select(x => x.OuterBounds.ToRect())
                .Aggregate((x, y) =>
                {
                    return (x, y) switch
                    {
                        _ when x.Bottom < y.Bottom => y,
                        _ when x.Bottom == y.Bottom && x.Right < y.Right => y,
                        _ => x
                    };
                });

            // Move to the bottom, right most corner of the lowest-positioned display with 1px still in view so the
            // window content is still rendered.
            AppWindow.Move(new PointInt32((int)lowestDisplayArea.Right - 1, (int)lowestDisplayArea.Bottom - 1));
        }

        private void DisplayWatcher_DisplayChanged(object? sender, EventArgs e) => RepositionToSharingPosition();

        private void ShareWindow_Activated(object sender, WindowActivatedEventArgs args)
            => _trackingTransformService.UpdateLayout();

        private void ShareWindow_Closed(object sender, WindowEventArgs args)
        {
            Activated -= ShareWindow_Activated;
            Closed -= ShareWindow_Closed;
            _displayWatcher.DisplayChanged -= DisplayWatcher_DisplayChanged;
            _displayWatcher.Dispose();
            _captureService.UnregisterFrameHandler(Preview);
            _trackingTransformService.StopTrackingTransforms();
            WeakReferenceMessenger.Default.UnregisterAll(this);
        }
    }
}
