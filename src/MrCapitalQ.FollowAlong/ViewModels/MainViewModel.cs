﻿using CommunityToolkit.Mvvm.ComponentModel;
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

        private void HotKeysService_HotKeyInvoked(object? sender, HotKeyInvokedEventArgs e)
        {
            if (e.HotKeyType is HotKeyType.StartStop)
            {
                if (!_captureService.IsStarted && SelectedDisplay is not null)
                {
                    var captureItem = SelectedDisplay.DisplayArea.CreateCaptureItem();
                    _captureService.StartCapture(new(captureItem, SelectedDisplay.DisplayArea));
                    WeakReferenceMessenger.Default.Send(new ZoomChanged(_zoom));
                }
                else if (_captureService.IsStarted)
                    _captureService.StopCapture();
            }
            else if (e.HotKeyType == HotKeyType.ZoomIn)
                WeakReferenceMessenger.Default.Send(new ZoomChanged(_zoom = Math.Min(_zoom + ZoomStepSize, MaxZoom)));
            else if (e.HotKeyType == HotKeyType.ZoomOut)
                WeakReferenceMessenger.Default.Send(new ZoomChanged(_zoom = Math.Max(_zoom - ZoomStepSize, MinZoom)));
        }
    }
}