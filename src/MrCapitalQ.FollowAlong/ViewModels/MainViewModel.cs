using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MrCapitalQ.FollowAlong.Core.Capture;
using MrCapitalQ.FollowAlong.Core.Display;
using MrCapitalQ.FollowAlong.Core.HotKeys;
using MrCapitalQ.FollowAlong.Messages;
using System.Collections.ObjectModel;

namespace MrCapitalQ.FollowAlong.ViewModels;

internal partial class MainViewModel : ObservableObject
{
    private const double MaxZoom = 3;
    private const double MinZoom = 1;
    private const double ZoomStepSize = 0.5;
    private readonly IBitmapCaptureService _captureService;
    private readonly IScreenshotService _screenshotService;
    private readonly IDisplayCaptureItemCreator _displayCaptureItemCreator;
    private readonly IMessenger _messenger;

    private double _zoom = 1.5;

    [ObservableProperty]
    private ObservableCollection<DisplayViewModel> _displays = [];

    [ObservableProperty]
    private DisplayViewModel? _selectedDisplay;

    [ObservableProperty]
    private bool _showSessionControls;

    [ObservableProperty]
    private double _sessionControlsOpacity = 0.5;

    [ObservableProperty]
    private bool _isTrackingEnabled = true;

    [ObservableProperty]
    private ObservableCollection<AlertViewModel> _alerts = [];

    public MainViewModel(IDisplayService displayService,
        IHotKeysService hotKeysService,
        IBitmapCaptureService captureService,
        IScreenshotService screenshotService,
        IDisplayCaptureItemCreator displayCaptureItemCreator,
        IMessenger messenger)
    {
        _captureService = captureService;
        _screenshotService = screenshotService;
        _displayCaptureItemCreator = displayCaptureItemCreator;
        _messenger = messenger;

        hotKeysService.HotKeyInvoked += HotKeysService_HotKeyInvoked;
        hotKeysService.HotKeyRegistrationFailed += HotKeysService_HotKeyRegistrationFailed;

        Displays = new(displayService.GetAll().Select(x => new DisplayViewModel(x, _screenshotService)));
        SelectedDisplay = Displays.FirstOrDefault(x => x.DisplayItem.IsPrimary);

        _messenger.Register<MainViewModel, StopCapture>(this, (r, m) => r.Stop());
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

    [RelayCommand]
    private void Start()
    {
        if (_captureService.IsStarted || SelectedDisplay is null)
            return;

        _captureService.StartCapture(_displayCaptureItemCreator.Create(SelectedDisplay.DisplayItem));
        _messenger.Send(new ZoomChanged(Zoom));
        ShowSessionControls = true;
    }

    [RelayCommand]
    private void Stop()
    {
        if (!_captureService.IsStarted)
            return;

        foreach (var display in Displays)
            _ = display.LoadThumbnailAsync();

        _captureService.StopCapture();
        ShowSessionControls = false;
    }

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

    private void HotKeysService_HotKeyInvoked(object? sender, HotKeyInvokedEventArgs e)
    {
        if (e.HotKeyType is HotKeyType.StartStop)
        {
            if (!_captureService.IsStarted)
                Start();
            else
                Stop();
        }
        else if (e.HotKeyType == HotKeyType.ZoomIn)
            ZoomIn();
        else if (e.HotKeyType == HotKeyType.ZoomOut)
            ZoomOut();
        else if (e.HotKeyType == HotKeyType.ToggleTracking)
            IsTrackingEnabled = !IsTrackingEnabled;
    }

    private void HotKeysService_HotKeyRegistrationFailed(object? sender, HotKeyRegistrationFailedEventArgs e)
    {
        var shortcutTypeDisplayName = e.HotKeyType switch
        {
            HotKeyType.StartStop => "Start and stop",
            HotKeyType.ZoomIn => "Zoom in",
            HotKeyType.ZoomOut => "Zoom out",
            HotKeyType.ResetZoom => "Reset zoom",
            HotKeyType.ToggleTracking => "Pause and resume tracking",
            _ => $"HotKeyType {e.HotKeyType}"
        };

        Alerts.Add(AlertViewModel.Warning($"{shortcutTypeDisplayName} keyboard shortcut could not be registered."));
    }

    partial void OnIsTrackingEnabledChanged(bool value) => _messenger.Send(new TrackingToggled(value));
}
