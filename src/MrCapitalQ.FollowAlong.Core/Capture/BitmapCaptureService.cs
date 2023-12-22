﻿using Microsoft.Extensions.Logging;
using Microsoft.Graphics.Canvas;
using System;
using Windows.Foundation;
using Windows.Graphics;
using Windows.Graphics.Capture;
using Windows.Graphics.DirectX;

namespace MrCapitalQ.FollowAlong.Core.Capture
{
    public sealed class BitmapCaptureService : IDisposable
    {
        private readonly ILogger<BitmapCaptureService> _logger;
        private IBitmapFrameHandler? _handler;
        private CanvasDevice? _canvasDevice;
        private Direct3D11CaptureFramePool? _framePool;
        private GraphicsCaptureSession? _session;
        private SizeInt32 _lastSize;

        public BitmapCaptureService(ILogger<BitmapCaptureService> logger)
        {
            _logger = logger;
        }

        public bool IsStarted { get; private set; }

        public void StartCapture(GraphicsCaptureItem captureItem, IBitmapFrameHandler handler)
        {
            if (IsStarted)
                throw new InvalidOperationException("Cannot start capture because a capture is has already been started.");

            _logger.LogInformation("Starting capture session of {CaptureItemDisplayName}.", captureItem.DisplayName);

            IsStarted = true;

            _handler = handler;

            _canvasDevice = new CanvasDevice();
            _handler.Initialize(_canvasDevice, new Size(captureItem.Size.Width, captureItem.Size.Height));

            _framePool = Direct3D11CaptureFramePool.Create(_canvasDevice,
                DirectXPixelFormat.B8G8R8A8UIntNormalized,
                2,
                captureItem.Size);

            _framePool.FrameArrived += FramePool_FrameArrived;
            captureItem.Closed += (_, _) => StopCapture();

            _session = _framePool.CreateCaptureSession(captureItem);
            _session.StartCapture();
        }

        public void StopCapture()
        {
            _logger.LogInformation("Stopping capture session.");

            IsStarted = false;
            _handler?.ClearFrame();
            _handler = null;
            _canvasDevice?.Dispose();
            _canvasDevice = null;
            _framePool?.Dispose();
            _framePool = null;
            _session?.Dispose();
        }

        public void Dispose() => StopCapture();

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
                _handler?.HandleFrame(canvasBitmap);
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

                        _handler?.Initialize(_canvasDevice);
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
