using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MrCapitalQ.FollowAlong.Infrastructure.HotKeys;
using MrCapitalQ.FollowAlong.Messages;

namespace MrCapitalQ.FollowAlong.ViewModels;

public partial class PreviewViewModel : ObservableObject
{
    private const double MaxZoom = 3;
    private const double MinZoom = 1;
    private const double ZoomStepSize = 0.5;
    private readonly IMessenger _messenger;

    private double _zoom = 1.5;

    [ObservableProperty]
    private double _sessionControlsOpacity = 0.5;

    [ObservableProperty]
    private bool _isTrackingEnabled = true;

    public PreviewViewModel(IHotKeysService hotKeysService, IMessenger messenger)
    {
        hotKeysService.HotKeyInvoked += HotKeysService_HotKeyInvoked;

        _messenger = messenger;
        _messenger.Register<PreviewViewModel, StartCapture>(this, (r, m) => BroadcastCurrentState());

        BroadcastCurrentState();
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
    private void Stop() => _messenger.Send(StopCapture.Empty);

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

    private void BroadcastCurrentState()
    {
        _messenger.Send(new ZoomChanged(Zoom));
        _messenger.Send(new TrackingToggled(IsTrackingEnabled));
    }

    private void HotKeysService_HotKeyInvoked(object? sender, HotKeyInvokedEventArgs e)
    {
        if (e.HotKeyType == HotKeyType.ZoomIn)
            ZoomIn();
        else if (e.HotKeyType == HotKeyType.ZoomOut)
            ZoomOut();
        else if (e.HotKeyType == HotKeyType.ToggleTracking)
            IsTrackingEnabled = !IsTrackingEnabled;
    }

    partial void OnIsTrackingEnabledChanged(bool value) => _messenger.Send(new TrackingToggled(value));
}
