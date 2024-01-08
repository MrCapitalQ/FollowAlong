using Microsoft.Extensions.Logging;
using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.Graphics;
using Windows.Graphics.Capture;
using Windows.Graphics.DirectX;

namespace MrCapitalQ.FollowAlong.Core.Capture
{
    public sealed class BitmapCaptureService : IDisposable
    {
        public event EventHandler? Started;
        public event EventHandler? Stopped;

        private readonly ILogger<BitmapCaptureService> _logger;
        private readonly HashSet<IBitmapFrameHandler> _handlers = new();
        private GraphicsCaptureItem? _captureItem;
        private CanvasDevice? _canvasDevice;
        private Direct3D11CaptureFramePool? _framePool;
        private GraphicsCaptureSession? _session;
        private SizeInt32 _lastSize;

        public BitmapCaptureService(ILogger<BitmapCaptureService> logger)
        {
            _logger = logger;
        }

        public bool IsStarted { get; private set; }

        public void StartCapture(GraphicsCaptureItem captureItem)
        {
            if (IsStarted)
                throw new InvalidOperationException("Cannot start capture because a capture is has already been started.");

            _logger.LogInformation("Starting capture session of {CaptureItemDisplayName}.", captureItem.DisplayName);

            IsStarted = true;
            _captureItem = captureItem;

            _canvasDevice = new CanvasDevice();
            foreach (var handler in _handlers)
            {
                handler.Initialize(_canvasDevice, new Size(captureItem.Size.Width, captureItem.Size.Height));
            }

            _framePool = Direct3D11CaptureFramePool.Create(_canvasDevice,
                DirectXPixelFormat.B8G8R8A8UIntNormalized,
                2,
                captureItem.Size);

            _framePool.FrameArrived += FramePool_FrameArrived;
            captureItem.Closed += (_, _) => StopCapture();

            _session = _framePool.CreateCaptureSession(captureItem);
            _session.StartCapture();

            OnStarted();
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

            _canvasDevice?.Dispose();
            _canvasDevice = null;
            _framePool?.Dispose();
            _framePool = null;
            _session?.Dispose();
            _session = null;
            _captureItem = null;

            if (isStarted)
                OnStopped();
        }

        public void RegisterFrameHandler(IBitmapFrameHandler handler)
        {
            _handlers.Add(handler);

            if (IsStarted && _captureItem is not null && _canvasDevice is not null)
                handler.Initialize(_canvasDevice, new Size(_captureItem.Size.Width, _captureItem.Size.Height));
        }

        public void Dispose() => StopCapture();

        private void OnStarted()
        {
            var raiseEvent = Started;
            raiseEvent?.Invoke(this, new());
        }

        private void OnStopped()
        {
            var raiseEvent = Stopped;
            raiseEvent?.Invoke(this, new());
        }

        private void FramePool_FrameArrived(Direct3D11CaptureFramePool sender, object args)
        {
            var needsReset = false;

            using var frame = sender.TryGetNextFrame();
            if (frame is null)
                return;

            if (frame.ContentSize.Width != _lastSize.Width || frame.ContentSize.Height != _lastSize.Height)
            {
                needsReset = true;
                _lastSize = frame.ContentSize;
            }

            try
            {
                var canvasBitmap = CanvasBitmap.CreateFromDirect3D11Surface(_canvasDevice, frame.Surface);
                foreach (var handler in _handlers)
                {
                    handler.HandleFrame(canvasBitmap);
                }
            }
            catch (Exception ex) when (_canvasDevice?.IsDeviceLost(ex.HResult) != false)
            {
                ResetFramePool(frame.ContentSize);
            }
            catch (Exception ex) when (ex is ObjectDisposedException)
            {
                if (_framePool is not null)
                    _framePool.FrameArrived -= FramePool_FrameArrived;
                StopCapture();
            }

            if (needsReset)
                ResetFramePool(frame.ContentSize);
        }

        private void ResetFramePool(SizeInt32 size)
        {
            var recreateDevice = false;
            while (_canvasDevice == null)
            {
                try
                {
                    if (recreateDevice)
                    {
                        _canvasDevice = new CanvasDevice();
                        foreach (var handler in _handlers)
                        {
                            handler.Initialize(_canvasDevice);
                        };
                    }

                    _framePool?.Recreate(_canvasDevice,
                        DirectXPixelFormat.B8G8R8A8UIntNormalized,
                        2,
                        size);
                }
                catch (Exception ex) when (_canvasDevice?.IsDeviceLost(ex.HResult) != false)
                {
                    _canvasDevice = null;
                    recreateDevice = true;
                }
            }
        }
    }
}
