using MrCapitalQ.FollowAlong.Core.Utils;
using System;
using Windows.System;

namespace MrCapitalQ.FollowAlong.Core.HotKeys
{
    public sealed class HotKeysService : IDisposable
    {
        public event EventHandler<HotKeyInvokedEventArgs>? HotKeyInvoked;

        private const uint WM_HOTKEY = 0x0312;
        private const ModifierKeys HotKeyModifiers = ModifierKeys.Control | ModifierKeys.Shift | ModifierKeys.Alt;

        private readonly IWindowMessageMonitor _windowMessageMonitor;
        private IntPtr? _hwnd;

        public HotKeysService(IWindowMessageMonitor windowMessageMonitor)
        {
            _windowMessageMonitor = windowMessageMonitor;
        }

        public void RegisterHotKeys(IntPtr hwnd)
        {
            if (_hwnd.HasValue)
                throw new InvalidOperationException($"This service can only be registered to one window at a time.");

            _hwnd = hwnd;

            HotKeyInterops.RegisterHotKey(_hwnd.Value, (int)HotKeyType.StartStop, (uint)HotKeyModifiers, (uint)VirtualKey.F);
            HotKeyInterops.RegisterHotKey(_hwnd.Value, (int)HotKeyType.ZoomIn, (uint)HotKeyModifiers, (uint)AdditionalKeys.Plus);
            HotKeyInterops.RegisterHotKey(_hwnd.Value, (int)HotKeyType.ZoomOut, (uint)HotKeyModifiers, (uint)AdditionalKeys.Minus);

            _windowMessageMonitor.Init(hwnd);
            _windowMessageMonitor.WindowMessageReceived += WindowMessageMonitor_WindowMessageReceived;
        }

        public void Unregister()
        {
            if (_hwnd.HasValue)
            {
                HotKeyInterops.UnregisterHotKey(_hwnd.Value, 0);
                _hwnd = null;
            }
            _windowMessageMonitor.Reset();
        }

        public void Dispose()
        {
            Unregister();
            _windowMessageMonitor.WindowMessageReceived -= WindowMessageMonitor_WindowMessageReceived;
        }

        private void OnHotKeyInvoked(HotKeyInvokedEventArgs e)
        {
            var raiseEvent = HotKeyInvoked;
            raiseEvent?.Invoke(this, e);
        }

        private void WindowMessageMonitor_WindowMessageReceived(object? sender, WindowMessageEventArgs e)
        {
            if (e.MessageId != WM_HOTKEY)
                return;

            OnHotKeyInvoked(new((HotKeyType)e.WParam));
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

        private enum AdditionalKeys : uint
        {
            Plus = 187,
            Minus = 189
        }
    }
}
