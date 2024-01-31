using Microsoft.UI.Xaml;
using System;
using WinUIEx.Messaging;

namespace MrCapitalQ.FollowAlong.Core.HotKeys
{
    public sealed class HotKeysService : IDisposable
    {
        public event EventHandler<HotKeyInvokedEventArgs>? HotKeyInvoked;

        private const uint WM_HOTKEY = 0x0312;
        private const ModifierKeys HotKeyModifiers = ModifierKeys.Control | ModifierKeys.Shift | ModifierKeys.Alt;
        private IntPtr? _hwnd;
        private WindowMessageMonitor? _monitor;

        public void RegisterHotKeys(Window window)
        {
            if (_hwnd.HasValue)
                throw new InvalidOperationException($"This service can only be registered to one window at a time.");

            _hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);

            HotKeyInterops.RegisterHotKey(_hwnd.Value, (int)HotKeyType.StartStop, (uint)HotKeyModifiers, (uint)Keys.S);
            HotKeyInterops.RegisterHotKey(_hwnd.Value, (int)HotKeyType.ZoomIn, (uint)HotKeyModifiers, (uint)Keys.Plus);
            HotKeyInterops.RegisterHotKey(_hwnd.Value, (int)HotKeyType.ZoomOut, (uint)HotKeyModifiers, (uint)Keys.Minus);

            _monitor = new WindowMessageMonitor(_hwnd.Value);
            _monitor.WindowMessageReceived += Monitor_WindowMessageReceived;
        }

        public void UnregisterHotKey() => Dispose();

        public void Dispose()
        {
            if (_hwnd.HasValue)
            {
                HotKeyInterops.UnregisterHotKey(_hwnd.Value, 0);
                _hwnd = null;
            }

            if (_monitor is not null)
            {
                _monitor.WindowMessageReceived -= Monitor_WindowMessageReceived;
                _monitor.Dispose();
                _monitor = null;
            }
        }

        private void OnHotKeyInvoked(HotKeyInvokedEventArgs e)
        {
            var raiseEvent = HotKeyInvoked;
            raiseEvent?.Invoke(this, e);
        }

        private void Monitor_WindowMessageReceived(object? sender, WindowMessageEventArgs e)
        {
            if (e.Message.MessageId != WM_HOTKEY)
                return;

            OnHotKeyInvoked(new((HotKeyType)e.Message.WParam));
        }

        [Flags]
        private enum ModifierKeys
        {
            None = 0,
            Alt = 1,
            Control = 2,
            Shift = 4,
            WinKey = 8
        }

        private enum Keys : uint
        {
            S = 0x53,
            Plus = 0xBB,
            Minus = 0xBD
        }
    }
}
