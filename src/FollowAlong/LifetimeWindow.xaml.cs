using CommunityToolkit.Mvvm.Messaging;
using H.NotifyIcon.EfficiencyMode;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using MrCapitalQ.FollowAlong.Core.Keyboard;
using MrCapitalQ.FollowAlong.Infrastructure;
using MrCapitalQ.FollowAlong.Infrastructure.Capture;
using MrCapitalQ.FollowAlong.Infrastructure.Display;
using MrCapitalQ.FollowAlong.Infrastructure.Tracking;
using MrCapitalQ.FollowAlong.Infrastructure.Utils;
using MrCapitalQ.FollowAlong.Messages;
using System.Diagnostics.CodeAnalysis;
using WinRT.Interop;

namespace MrCapitalQ.FollowAlong;

[ExcludeFromCodeCoverage(Justification = ExcludeFromCoverageJustifications.RequiresUIThread)]
public sealed partial class LifetimeWindow : Window
{
    private readonly IShortcutService _shortcutService;
    private readonly IBitmapCaptureService _captureService;
    private readonly IMessenger _messenger;
    private readonly IDisplayService _displayService;
    private readonly IDisplayCaptureItemCreator _displayCaptureItemCreator;
    private readonly IPointerService _pointerService;

    private PreviewWindow? _previewWindow;
    private ShareWindow? _shareWindow;
    private bool _reshowMainWindow = false;

    public LifetimeWindow(IShortcutService shortcutService,
        IBitmapCaptureService captureService,
        IMessenger messenger,
        IDisplayService displayService,
        IDisplayCaptureItemCreator displayCaptureItemCreator,
        IPointerService pointerService)
    {
        _shortcutService = shortcutService;
        _captureService = captureService;
        _messenger = messenger;
        _displayService = displayService;
        _displayCaptureItemCreator = displayCaptureItemCreator;
        _pointerService = pointerService;

        InitializeComponent();

        RegisterShortcuts();
        _shortcutService.ShortcutInvoked += ShortcutService_ShortcutInvoked;

        _messenger.Register<LifetimeWindow, StartCapture>(this, (r, m) =>
        {
            EfficiencyModeUtilities.SetEfficiencyMode(false);

            if (m.CaptureItem is IDisplayCaptureItem captureItem)
            {
                r.Start(captureItem);
                return;
            }

            var currentPointerPosition = r._pointerService.GetCurrentPosition();
            if (currentPointerPosition is null)
                return;

            var currentPointerDisplay = r._displayService.GetAll()
                .FirstOrDefault(x => x.OuterBounds.ToRect().Contains(currentPointerPosition.Value));
            if (currentPointerDisplay is null)
                return;

            r.Start(r._displayCaptureItemCreator.Create(currentPointerDisplay));
        });
        _messenger.Register<LifetimeWindow, StopCapture>(this, (r, m) =>
        {
            EfficiencyModeUtilities.SetEfficiencyMode(true);
            r.Stop();
        });

        _messenger.Register<LifetimeWindow, RegisterShortcutsMessage>(this, (r, m) => r.RegisterShortcuts());
        _messenger.Register<LifetimeWindow, UnregisterShortcutsMessage>(this, (r, m) => r._shortcutService.Unregister());

        Closed += LifetimeWindow_Closed;

        TrayIcon.ToolTipText = Windows.ApplicationModel.Package.Current.GetAppListEntries()[0].DisplayInfo.DisplayName;
    }

    private void Start(IDisplayCaptureItem captureItem)
    {
        if (_captureService.IsStarted)
            return;

        if (App.Current.MainWindow is MainWindow mainWindow)
        {
            mainWindow.Close();
            _reshowMainWindow = true;
        }

        if (_previewWindow is null)
        {
            _previewWindow = App.Current.Services.GetRequiredService<PreviewWindow>();
            _previewWindow.Closed += PreviewWindow_Closed;
            _previewWindow.Activate();
        }

        if (_shareWindow is null)
        {
            _shareWindow = App.Current.Services.GetRequiredService<ShareWindow>();
            _shareWindow.Closed += ShareWindow_Closed;
            _shareWindow.Activate();
        }

        _captureService.StartCapture(captureItem);
        _shareWindow.SetScreenSize(captureItem.OuterBounds.ToSizeInt32());
    }

    private void Stop()
    {
        _captureService.StopCapture();

        if (_reshowMainWindow)
        {
            App.Current.ShowMainWindow();
            _reshowMainWindow = false;
        }

        if (_previewWindow is not null)
        {
            _previewWindow.Closed -= PreviewWindow_Closed;
            _previewWindow.Close();
            _previewWindow = null;
        }

        if (_shareWindow is not null)
        {
            _shareWindow.Closed -= ShareWindow_Closed;
            _shareWindow.Close();
            _shareWindow = null;
        }
    }

    private void RegisterShortcuts() => _shortcutService.Register(WindowNative.GetWindowHandle(this));

    private void ShortcutService_ShortcutInvoked(object? sender, AppShortcutInvokedEventArgs e)
    {
        if (e.ShortcutKind is not AppShortcutKind.StartStop)
            return;

        if (!_captureService.IsStarted)
            _messenger.Send(StartCapture.Empty);
        else
            _messenger.Send(StopCapture.Instance);
    }

    private void LifetimeWindow_Closed(object sender, WindowEventArgs args)
    {
        _reshowMainWindow = false;
        Stop();
    }

    private void PreviewWindow_Closed(object sender, WindowEventArgs args) => Stop();

    private void ShareWindow_Closed(object sender, WindowEventArgs args) => Stop();

    private void OpenApplicationCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        => App.Current.ShowMainWindow();

    private void StartSessionCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        => _messenger.Send(StartCapture.Empty);

    private void ExitApplicationCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        => App.Current.Exit();
}
