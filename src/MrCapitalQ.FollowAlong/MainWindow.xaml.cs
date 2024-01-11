using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
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
        private const double ViewportAspectRatio = 16 / 9d;
        private readonly static SizeInt32 s_defaultWindowSize = new(800, 600);

        private readonly TrackingTransformService _trackingTransformService;
        private readonly MainViewModel _viewModel;
        private PreviewWindow? _previewWindow;

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
            SetDefaultWindowSizeAndPosition(s_defaultWindowSize);

            Root.Loaded += Root_Loaded;
            Closed += MainWindow_Closed;
        }

        private void SetDefaultWindowSizeAndPosition(SizeInt32 size)
        {
            var scale = Root.XamlRoot?.RasterizationScale ?? 1;
            AppWindow.Resize(new((int)(size.Width * scale), (int)(size.Height * scale)));
            this.CenterOnScreen();
        }

        private void SetViewportWindowSizeAndPosition(CaptureStartedEventArgs e)
        {
            this.Restore();
            var viewportSize = (e.Size.Width / e.Size.Height) > ViewportAspectRatio
                            ? new SizeInt32((int)(e.Size.Height * ViewportAspectRatio), e.Size.Height)
                            : new SizeInt32(e.Size.Width, (int)(e.Size.Width / ViewportAspectRatio));
            AppWindow.Resize(viewportSize);

            var appMonitor = this.GetWindowMonitorSize();
            if (appMonitor is not null)
                AppWindow.Move(new PointInt32((int)appMonitor.ScreenSize.X - 1, (int)appMonitor.ScreenSize.Y - 1));
        }

        private void CaptureService_Started(object? sender, CaptureStartedEventArgs e)
        {
            MainContent.Visibility = Visibility.Collapsed;

            SetViewportWindowSizeAndPosition(e);

            this.SetIsResizable(false);
            this.SetIsMinimizable(false);
            this.SetIsMaximizable(false);

            if (_previewWindow is null)
            {
                _previewWindow = App.Current.Services.GetRequiredService<PreviewWindow>();
                _previewWindow.Closed += PreviewWindow_Closed;
            }
            _previewWindow.Activate();
        }

        private void CaptureService_Stopped(object? sender, EventArgs e)
        {
            MainContent.Visibility = Visibility.Visible;

            SetDefaultWindowSizeAndPosition(s_defaultWindowSize);
            this.SetIsResizable(true);
            this.SetIsMinimizable(true);
            this.SetIsMaximizable(true);
            this.SetForegroundWindow();

            if (_previewWindow is not null)
            {
                _previewWindow.Closed -= PreviewWindow_Closed;
                _previewWindow.Close();
                _previewWindow = null;
            }
        }

        private void MainWindow_Closed(object sender, WindowEventArgs args) => _previewWindow?.Close();

        private void Root_Loaded(object sender, RoutedEventArgs e)
        {
            Root.Loaded -= Root_Loaded;
            SetDefaultWindowSizeAndPosition(s_defaultWindowSize);
        }

        private void PreviewWindow_Closed(object sender, WindowEventArgs args)
        {
            if (_previewWindow is null)
                return;

            _previewWindow.Closed -= PreviewWindow_Closed;
            _previewWindow = null;
        }
    }
}
