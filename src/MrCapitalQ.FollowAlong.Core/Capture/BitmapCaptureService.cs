using Microsoft.Extensions.Logging;
using MrCapitalQ.FollowAlong.Core.Utils;
using System;
using System.Collections.Generic;
using Windows.Graphics;

namespace MrCapitalQ.FollowAlong.Core.Capture;

public sealed class BitmapCaptureService : IBitmapCaptureService, IDisposable
{
    public event EventHandler<CaptureStartedEventArgs>? Started;
    public event EventHandler? Stopped;

    private readonly ICaptureSessionAdapter _captureSessionAdapter;
    private readonly ILogger<BitmapCaptureService> _logger;
    private readonly HashSet<IBitmapFrameHandler> _handlers = [];
    private IDisplayCaptureItem? _captureItem;

    public BitmapCaptureService(ICaptureSessionAdapter captureSessionAdapter, ILogger<BitmapCaptureService> logger)
    {
        _captureSessionAdapter = captureSessionAdapter;
        _captureSessionAdapter.FrameArrived += CaptureSessionAdapter_FrameArrived;
        _captureSessionAdapter.Recreated += CaptureSessionAdapter_Recreated;

        _logger = logger;
    }

    public bool IsStarted { get; private set; }

    public void StartCapture(IDisplayCaptureItem captureItem)
    {
        if (IsStarted)
            throw new InvalidOperationException("Cannot start capture because a capture is has already been started.");

        _logger.LogInformation("Starting capture session of {CaptureItemDisplayName}.", captureItem.DisplayName);

        IsStarted = true;
        _captureItem = captureItem;

        captureItem.Closed += (_, _) => StopCapture();

        _captureSessionAdapter.Start(captureItem);

        if (_captureSessionAdapter.CanvasDevice is not null)
            foreach (var handler in _handlers)
            {
                handler.Initialize(_captureSessionAdapter.CanvasDevice, captureItem.OuterBounds);
            }

        OnStarted(captureItem.OuterBounds.ToSizeInt32());
    }

    public void StopCapture()
    {
        var isStarted = IsStarted;

        if (isStarted)
            _logger.LogInformation("Stopping capture session.");

        IsStarted = false;

        foreach (var handler in _handlers)
        {
            handler.Stop();
        }

        _captureSessionAdapter.Stop();

        _captureItem = null;

        if (isStarted)
            OnStopped();
    }

    public void RegisterFrameHandler(IBitmapFrameHandler handler)
    {
        _handlers.Add(handler);

        if (IsStarted && _captureItem is not null && _captureSessionAdapter.CanvasDevice is not null)
            handler.Initialize(_captureSessionAdapter.CanvasDevice, _captureItem.OuterBounds);
    }

    public void UnregisterFrameHandler(IBitmapFrameHandler handler)
    {
        _handlers.Remove(handler);
        handler.Stop();
    }

    public void Dispose() => StopCapture();

    private void OnStarted(SizeInt32 size)
    {
        var raiseEvent = Started;
        raiseEvent?.Invoke(this, new(size));
    }

    private void OnStopped()
    {
        var raiseEvent = Stopped;
        raiseEvent?.Invoke(this, new());
    }

    private void CaptureSessionAdapter_FrameArrived(object? sender, FrameArrivedEventArgs e)
    {
        foreach (var handler in _handlers)
        {
            handler.HandleFrame(e.Bitmap);
        }
    }

    private void CaptureSessionAdapter_Recreated(object? sender, EventArgs e)
    {
        if (_captureSessionAdapter.CanvasDevice is null)
            return;

        foreach (var handler in _handlers)
        {
            handler.Initialize(_captureSessionAdapter.CanvasDevice);
        };
    }
}
