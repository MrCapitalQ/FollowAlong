using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using MrCapitalQ.FollowAlong.Core.Capture;
using MrCapitalQ.FollowAlong.Core.HotKeys;
using MrCapitalQ.FollowAlong.Core.Monitors;
using MrCapitalQ.FollowAlong.Core.Tracking;
using MrCapitalQ.FollowAlong.Core.Utils;
using System;
using Windows.Graphics;
using WinUIEx;

namespace MrCapitalQ.FollowAlong
{
    internal sealed partial class MainWindow : Window
    {
        private const int BottomPadding = 48;
        private readonly static SizeInt32 s_defaultWindowSize = new(800, 600);
        private readonly static SizeInt32 s_previewWindowSize = new(384, 216);

        private readonly TrackingTransformService _trackingTransformService;
        private readonly MainViewModel _viewModel;
        private ShareWindow? _shareWindow;

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
            _trackingTransformService.StartTrackingTransforms(Preview);
            WeakReferenceMessenger.Default.Register<ZoomChanged>(this,
                (r, m) => _trackingTransformService.Zoom = m.Zoom);

            hotKeysService.RegisterHotKeys(this);

            _viewModel = viewModel;

            ExtendsContentIntoTitleBar = true;
            SetWindowToDefaultMode();

            Root.Loaded += Root_Loaded;
            Closed += MainWindow_Closed;
        }

        private void SetWindowToDefaultMode()
        {
            Title = "Follow Along";
            MainContent.Visibility = Visibility.Visible;

            this.SetIsResizable(true);
            this.SetIsMaximizable(true);
            this.SetIsAlwaysOnTop(false);
            this.SetIsExcludedFromCapture(false);

            var scale = Root.XamlRoot?.RasterizationScale ?? 1;
            AppWindow.Resize(new((int)(s_defaultWindowSize.Width * scale), (int)(s_defaultWindowSize.Height * scale)));
            this.CenterOnScreen();
        }

        private void SetWindowToPreviewMode()
        {
            // Teams does not list windows with no title. Set no title so the preview window cannot be selected.
            Title = null;
            MainContent.Visibility = Visibility.Collapsed;

            this.SetIsResizable(false);
            this.SetIsMaximizable(false);
            this.SetIsAlwaysOnTop(true);
            this.SetIsExcludedFromCapture(true);

            var scale = Root.XamlRoot?.RasterizationScale ?? 1;

            AppWindow.Resize(new((int)(s_previewWindowSize.Width * scale), (int)(s_previewWindowSize.Height * scale)));

            var appMonitor = this.GetWindowMonitorSize();
            if (appMonitor is not null)
                AppWindow.Move(new PointInt32(0,
                    (int)(appMonitor.ScreenSize.Y - (s_previewWindowSize.Height * scale) - (BottomPadding * scale))));
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
            MainContent.Visibility = Visibility.Visible;
            SetWindowToDefaultMode();

            this.SetForegroundWindow();

            if (_shareWindow is not null)
            {
                _shareWindow.Closed -= ShareWindow_Closed;
                _shareWindow.Close();
                _shareWindow = null;
            }
        }

        private void MainWindow_Closed(object sender, WindowEventArgs args) => _shareWindow?.Close();

        private void Root_Loaded(object sender, RoutedEventArgs e)
        {
            Root.Loaded -= Root_Loaded;
            SetWindowToDefaultMode();
        }

        private void ShareWindow_Closed(object sender, WindowEventArgs args)
        {
            if (_shareWindow is null)
                return;

            _shareWindow.Closed -= ShareWindow_Closed;
            _shareWindow = null;
        }
    }
}
