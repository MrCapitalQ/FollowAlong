﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MrCapitalQ.FollowAlong.Core.Keyboard;
using MrCapitalQ.FollowAlong.Messages;

namespace MrCapitalQ.FollowAlong.ViewModels;

public partial class PreviewViewModel : ObservableObject
{
    private const double DefaultSessionControlsOpacity = 0.5;
    private const double MaxZoom = 3;
    private const double MinZoom = 1;
    private const double ZoomStepSize = 0.5;
    private readonly IMessenger _messenger;

    private double _zoom = 1.5;

    [ObservableProperty]
    private double _sessionControlsOpacity = DefaultSessionControlsOpacity;

    [ObservableProperty]
    private bool _isTrackingEnabled = true;

    public PreviewViewModel(IShortcutService shortcutService, IMessenger messenger)
    {
        shortcutService.ShortcutInvoked += ShortcutService_ShortcutInvoked;

        _messenger = messenger;
        _messenger.Register<PreviewViewModel, StartCapture>(this, (r, m) => HandleStart());

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
