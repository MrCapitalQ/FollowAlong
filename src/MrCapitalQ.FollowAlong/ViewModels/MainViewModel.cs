using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MrCapitalQ.FollowAlong.Core.Capture;
using MrCapitalQ.FollowAlong.Core.Display;
using MrCapitalQ.FollowAlong.Core.HotKeys;
using MrCapitalQ.FollowAlong.Messages;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace MrCapitalQ.FollowAlong.ViewModels
{
    internal partial class MainViewModel : ObservableObject
    {
        private const double MaxZoom = 3;
        private const double MinZoom = 1;
        private const double ZoomStepSize = 0.5;
        private readonly BitmapCaptureService _captureService;
        private double _zoom = 1.5;

        [ObservableProperty]
        private ObservableCollection<DisplayViewModel> _displays = new();

        [ObservableProperty]
        private DisplayViewModel? _selectedDisplay;

        public MainViewModel(DisplayService displayService,
            HotKeysService hotKeysService,
            BitmapCaptureService captureService)
        {
            hotKeysService.HotKeyInvoked += HotKeysService_HotKeyInvoked;

            Displays = new(displayService.GetAll().Select(x => new DisplayViewModel(x)));
            SelectedDisplay = Displays.FirstOrDefault(x => x.DisplayArea.IsPrimary);
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
            if (SelectedDisplay is null)
                return;

            var captureItem = SelectedDisplay.DisplayArea.CreateCaptureItem();
            _captureService.StartCapture(new(captureItem, SelectedDisplay.DisplayArea));
            WeakReferenceMessenger.Default.Send(new ZoomChanged(_zoom));
        }

        private void HotKeysService_HotKeyInvoked(object? sender, HotKeyInvokedEventArgs e)
        {
            if (e.HotKeyType is HotKeyType.StartStop)
            {
                if (!_captureService.IsStarted)
                    Start();
                else if (_captureService.IsStarted)
                {
                    foreach (var display in Displays)
                    {
                        _ = display.LoadThumbnailAsync();
                    }

                    _captureService.StopCapture();
                }
            }
            else if (e.HotKeyType == HotKeyType.ZoomIn)
                WeakReferenceMessenger.Default.Send(new ZoomChanged(_zoom = Math.Min(_zoom + ZoomStepSize, MaxZoom)));
            else if (e.HotKeyType == HotKeyType.ZoomOut)
                WeakReferenceMessenger.Default.Send(new ZoomChanged(_zoom = Math.Max(_zoom - ZoomStepSize, MinZoom)));
        }
    }
}
