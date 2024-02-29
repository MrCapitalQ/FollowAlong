using Microsoft.Graphics.Canvas;
using MrCapitalQ.FollowAlong.Core.Utils;
using System;
using System.Diagnostics.CodeAnalysis;
using Windows.Graphics;
using Windows.Graphics.Capture;
using Windows.Graphics.DirectX;

namespace MrCapitalQ.FollowAlong.Core.Capture
{
    [ExcludeFromCodeCoverage(Justification = "Adapter class for native graphics classes that can't be unit tested.")]
    public sealed class CaptureSessionAdapter : ICaptureSessionAdapter, IDisposable
    {
        public event EventHandler<FrameArrivedEventArgs>? FrameArrived;
        public event EventHandler? Recreated;

        private CanvasDevice? _canvasDevice;
        private Direct3D11CaptureFramePool? _framePool;
        private GraphicsCaptureSession? _session;
        private SizeInt32 _lastSize;

        public CanvasDevice? CanvasDevice => _canvasDevice;

        public void Start(IDisplayCaptureItem captureItem)
        {
            if (captureItem is not DisplayCaptureItem displayCaptureItem)
                return;

            _canvasDevice = new CanvasDevice();

            _framePool = Direct3D11CaptureFramePool.Create(_canvasDevice,
                DirectXPixelFormat.B8G8R8A8UIntNormalized,
                2,
                captureItem.OuterBounds.ToSizeInt32());

            _framePool.FrameArrived += FramePool_FrameArrived;

            _session = _framePool.CreateCaptureSession(displayCaptureItem.GraphicsCaptureItem);
            _session.StartCapture();
        }

        public void Stop()
        {
            _canvasDevice?.Dispose();
            _canvasDevice = null;
            _framePool?.Dispose();
            _framePool = null;
            _session?.Dispose();
            _session = null;
        }

        public void Dispose() => Stop();

        private void OnFrameArrived(CanvasBitmap bitmap)
        {
            var raiseEvent = FrameArrived;
            raiseEvent?.Invoke(this, new(bitmap));
        }

        private void OnRecreated()
        {
            var raiseEvent = Recreated;
            raiseEvent?.Invoke(this, new());
        }

        private void FramePool_FrameArrived(Direct3D11CaptureFramePool sender, object args)
        {
            var needsReset = false;

            using var frame = sender.TryGetNextFrame();

            if (frame.ContentSize.Width != _lastSize.Width || frame.ContentSize.Height != _lastSize.Height)
            {
                needsReset = true;
                _lastSize = frame.ContentSize;
            }

            try
            {
                var canvasBitmap = CanvasBitmap.CreateFromDirect3D11Surface(_canvasDevice, frame.Surface);
                OnFrameArrived(canvasBitmap);
            }
            catch (Exception ex) when (_canvasDevice?.IsDeviceLost(ex.HResult) != false)
            {
                ResetFramePool(frame.ContentSize);
            }
            catch (Exception ex) when (ex is ObjectDisposedException)
            {
                if (_framePool is not null)
                    _framePool.FrameArrived -= FramePool_FrameArrived;
                Stop();
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
                        OnRecreated();
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

    public class FrameArrivedEventArgs : EventArgs
    {
        public FrameArrivedEventArgs(CanvasBitmap bitmap) => Bitmap = bitmap;

        public CanvasBitmap Bitmap { get; }
    }
}
