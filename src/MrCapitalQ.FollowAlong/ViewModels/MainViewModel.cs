using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MrCapitalQ.FollowAlong.Core.Capture;
using MrCapitalQ.FollowAlong.Core.Display;
using MrCapitalQ.FollowAlong.Core.HotKeys;
using MrCapitalQ.FollowAlong.Core.Utils;
using MrCapitalQ.FollowAlong.Messages;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.Graphics.Capture;

namespace MrCapitalQ.FollowAlong.ViewModels
{
    internal partial class MainViewModel : ObservableObject
    {
        private const double MaxZoom = 3;
        private const double MinZoom = 1;
        private const double ZoomStepSize = 0.5;
        private readonly BitmapCaptureService _captureService;

        [ObservableProperty]
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

        public MainViewModel(DisplayService displayService,
            HotKeysService hotKeysService,
            BitmapCaptureService captureService)
        {
            hotKeysService.HotKeyInvoked += HotKeysService_HotKeyInvoked;
            hotKeysService.HotKeyRegistrationFailed += HotKeysService_HotKeyRegistrationFailed;

            Displays = new(displayService.GetAll().Select(x => new DisplayViewModel(x)));
            SelectedDisplay = Displays.FirstOrDefault(x => x.DisplayItem.IsPrimary);
            _captureService = captureService;

            WeakReferenceMessenger.Default.Register<StopCapture>(this,
                (r, m) =>
                {
                    if (_captureService.IsStarted)
                        _captureService.StopCapture();
                });
        }

        [RelayCommand]
        private void Start()
        {
            if (_captureService.IsStarted || SelectedDisplay is null)
                return;

            var captureItem = GraphicsCaptureItem.TryCreateFromDisplayId(new(SelectedDisplay.DisplayItem.DisplayId));
            if (captureItem is null)
                return;

            _captureService.StartCapture(new DisplayCaptureItem(captureItem, SelectedDisplay.DisplayItem.OuterBounds.ToRect()));
            WeakReferenceMessenger.Default.Send(new ZoomChanged(Zoom));
            ShowSessionControls = true;
        }

        [RelayCommand]
        private void Stop()
        {
            if (!_captureService.IsStarted)
                return;

            foreach (var display in Displays)
            {
                _ = display.LoadThumbnailAsync();
            }

            _captureService.StopCapture();
            ShowSessionControls = false;
        }

        [RelayCommand]
        private void ZoomIn()
        {
            WeakReferenceMessenger.Default.Send(new ZoomChanged(Zoom = Math.Min(Zoom + ZoomStepSize, MaxZoom)));
        }

        [RelayCommand]
        private void ZoomOut()
        {
            WeakReferenceMessenger.Default.Send(new ZoomChanged(Zoom = Math.Max(Zoom - ZoomStepSize, MinZoom)));
        }

        [RelayCommand]
        private void UpdateSessionControlOpacity(string parameterValue)
        {
            if (!double.TryParse(parameterValue, out var opacity))
                return;

            SessionControlsOpacity = opacity;
        }

        private void ToggleTracking()
        {
            IsTrackingEnabled = !IsTrackingEnabled;
            WeakReferenceMessenger.Default.Send(new TrackingToggled(IsTrackingEnabled));
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
                ToggleTracking();
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
                _ => e.HotKeyType.ToString()
            };

            if (shortcutTypeDisplayName is not null)
                Alerts.Add(AlertViewModel.Warning($"{shortcutTypeDisplayName} keyboard shortcut could not be registered."));
        }
    }
}
