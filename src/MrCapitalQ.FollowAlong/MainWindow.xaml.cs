using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using MrCapitalQ.FollowAlong.Core.Capture;
using MrCapitalQ.FollowAlong.Core.Display;
using MrCapitalQ.FollowAlong.Core.HotKeys;
using MrCapitalQ.FollowAlong.Core.Tracking;
using MrCapitalQ.FollowAlong.Core.Utils;
using MrCapitalQ.FollowAlong.Messages;
using System;
using Windows.Graphics;
using WinRT.Interop;
using WinUIEx;

namespace MrCapitalQ.FollowAlong
{
    internal sealed partial class MainWindow : WindowBase
    {
        private readonly static SizeInt32 s_minWindowSize = new(600, 450);
        private readonly static SizeInt32 s_previewWindowSize = new(384, 216);

        private readonly TrackingTransformService _trackingTransformService;
        private ShareWindow? _shareWindow;

        public MainWindow(BitmapCaptureService captureService,
            TrackingTransformService trackingTransformService,
            HotKeysService hotKeysService)
        {
            InitializeComponent();

            captureService.RegisterFrameHandler(Preview);
            captureService.Started += CaptureService_Started;
            captureService.Stopped += CaptureService_Stopped;

            _trackingTransformService = trackingTransformService;
            _trackingTransformService.StartTrackingTransforms(Preview);
            WeakReferenceMessenger.Default.Register<ZoomChanged>(this,
                (r, m) => _trackingTransformService.Zoom = m.Zoom);
            WeakReferenceMessenger.Default.Register<TrackingToggled>(this,
                (r, m) => _trackingTransformService.IsTrackingEnabled = m.IsEnabled);

            hotKeysService.RegisterHotKeys(WindowNative.GetWindowHandle(this));

            ExtendsContentIntoTitleBar = true;
            SetWindowToDefaultMode();
            this.CenterOnScreen();

            Closed += MainWindow_Closed;
        }

        private void SetWindowToDefaultMode()
        {
            Title = "Follow Along";
            TitleBar.Visibility = Visibility.Visible;

            PresenterKind = AppWindowPresenterKind.Default;
            MinWidth = s_minWindowSize.Width;
            MinHeight = s_minWindowSize.Height;
            this.SetIsExcludedFromCapture(false);
        }

        private void SetWindowToPreviewMode()
        {
            // Teams does not list windows with no title. Set no title so the preview window cannot be selected.
            Title = null!;
            TitleBar.Visibility = Visibility.Collapsed;

            PresenterKind = AppWindowPresenterKind.CompactOverlay;
            MinWidth = 0;
            MinHeight = 0;
            this.SetIsExcludedFromCapture(true);

            Width = s_previewWindowSize.Width;
            Height = s_previewWindowSize.Height;

            var displayItem = this.GetCurrentDisplayItem();
            if (displayItem is not null)
                AppWindow.Move(new PointInt32(displayItem.WorkArea.X,
                    displayItem.WorkArea.Y + displayItem.WorkArea.Height - AppWindow.Size.Height));
        }

        private void CaptureService_Started(object? sender, CaptureStartedEventArgs e)
        {
            SetWindowToPreviewMode();

            if (_shareWindow is null)
            {
                _shareWindow = App.Current.Services.GetRequiredService<ShareWindow>();
                _shareWindow.Closed += ShareWindow_Closed;
            }
            _shareWindow.Activate();
            _shareWindow.SetScreenSize(e.Size);
        }

        private void CaptureService_Stopped(object? sender, EventArgs e)
        {
            SetWindowToDefaultMode();

            this.SetForegroundWindow();

            _shareWindow?.Close();
        }

        private void MainWindow_Closed(object sender, WindowEventArgs args) => _shareWindow?.Close();

        private void ShareWindow_Closed(object sender, WindowEventArgs args)
        {
            if (sender is Window window)
                window.Closed -= ShareWindow_Closed;

            if (_shareWindow is null)
                return;

            _shareWindow.Closed -= ShareWindow_Closed;
            _shareWindow = null;

            WeakReferenceMessenger.Default.Send(new StopCapture());
        }
    }
}
