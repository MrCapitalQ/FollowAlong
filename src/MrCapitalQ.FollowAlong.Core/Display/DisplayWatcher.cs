using Microsoft.UI.Xaml;
using System;
using WinUIEx.Messaging;

namespace MrCapitalQ.FollowAlong.Core.Display
{
    public sealed class DisplayWatcher : IDisposable
    {
        public event EventHandler? DisplayChanged;

        private const uint WM_DISPLAYCHANGE = 0x07E;
        private IntPtr? _hwnd;
        private WindowMessageMonitor? _monitor;

        public void Register(Window window)
        {
            if (_hwnd.HasValue)
                throw new InvalidOperationException($"This service can only be registered to one window at a time.");

            _hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);

            _monitor = new WindowMessageMonitor(_hwnd.Value);
            _monitor.WindowMessageReceived += Monitor_WindowMessageReceived;
        }

        public void Unregister() => Dispose();

        public void Dispose()
        {
            _hwnd = null;

            if (_monitor is not null)
            {
                _monitor.WindowMessageReceived -= Monitor_WindowMessageReceived;
                _monitor.Dispose();
                _monitor = null;
            }
        }

        private void OnDisplayChanged()
        {
            var raiseEvent = DisplayChanged;
            raiseEvent?.Invoke(this, new());
        }

        private void Monitor_WindowMessageReceived(object? sender, WindowMessageEventArgs e)
        {
            if (e.Message.MessageId != WM_DISPLAYCHANGE)
                return;

            OnDisplayChanged();
        }
    }
}
