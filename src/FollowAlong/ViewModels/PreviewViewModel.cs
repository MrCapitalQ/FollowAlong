using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MrCapitalQ.FollowAlong.Core.AppData;
using MrCapitalQ.FollowAlong.Core.Keyboard;
using MrCapitalQ.FollowAlong.Messages;
using MrCapitalQ.FollowAlong.Shared;

namespace MrCapitalQ.FollowAlong.ViewModels;

public partial class PreviewViewModel : ObservableObject
{
    private const double DefaultSessionControlsOpacity = 0.5;
    private const double MaxZoom = 3;
    private const double MinZoom = 1;
    private const double ZoomStepSize = 0.5;

    private readonly IMessenger _messenger;
    private readonly ISettingsService _settingsService;

    private double _zoom;

    [ObservableProperty]
    private double _sessionControlsOpacity = DefaultSessionControlsOpacity;

    [ObservableProperty]
    private bool _isTrackingEnabled = true;

    public PreviewViewModel(IShortcutService shortcutService, IMessenger messenger, ISettingsService settingsService)
    {
        shortcutService.ShortcutInvoked += ShortcutService_ShortcutInvoked;

        _messenger = messenger;
        _settingsService = settingsService;
        _messenger.Register<PreviewViewModel, StartCapture>(this, (r, m) => HandleStart());

        _zoom = _settingsService.GetDefaultZoom();

        HandleStart();
    }

    public double Zoom
    {
        get => _zoom;
        set
        {
            _zoom = Math.Clamp(value, MinZoom, MaxZoom);
            OnPropertyChanged();
            _messenger.Send(new ZoomChanged(_zoom));
        }
    }

    public string StopToolTip => _settingsService.GetShortcutKeys(AppShortcutKind.StartStop).ToDisplayString();
    public string ZoomInToolTip => _settingsService.GetShortcutKeys(AppShortcutKind.ZoomIn).ToDisplayString();
    public string ZoomOutToolTip => _settingsService.GetShortcutKeys(AppShortcutKind.ZoomOut).ToDisplayString();

    [RelayCommand]
    private void Stop() => _messenger.Send(StopCapture.Instance);

    [RelayCommand]
    private void ZoomIn() => Zoom += ZoomStepSize;

    [RelayCommand]
    private void ZoomOut() => Zoom -= ZoomStepSize;

    [RelayCommand]
    private void UpdateSessionControlOpacity(string parameterValue)
    {
        if (!double.TryParse(parameterValue, out var opacity))
            return;

        SessionControlsOpacity = opacity;
    }

    private void HandleStart()
    {
        SessionControlsOpacity = DefaultSessionControlsOpacity;
        _messenger.Send(new ZoomChanged(Zoom));
        _messenger.Send(new TrackingToggled(IsTrackingEnabled));
    }

    private void ShortcutService_ShortcutInvoked(object? sender, AppShortcutInvokedEventArgs e)
    {
        if (e.ShortcutKind == AppShortcutKind.ZoomIn)
            ZoomIn();
        else if (e.ShortcutKind == AppShortcutKind.ZoomOut)
            ZoomOut();
        else if (e.ShortcutKind == AppShortcutKind.ToggleTracking)
            IsTrackingEnabled = !IsTrackingEnabled;
    }

    partial void OnIsTrackingEnabledChanged(bool value) => _messenger.Send(new TrackingToggled(value));
}
