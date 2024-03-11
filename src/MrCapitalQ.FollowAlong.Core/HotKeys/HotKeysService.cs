using MrCapitalQ.FollowAlong.Core.Utils;
using System;
using System.Collections.Generic;
using Windows.System;

namespace MrCapitalQ.FollowAlong.Core.HotKeys
{
    public sealed class HotKeysService : IDisposable
    {
        public event EventHandler<HotKeyInvokedEventArgs>? HotKeyInvoked;
        public event EventHandler<HotKeyRegistrationFailedEventArgs>? HotKeyRegistrationFailed;

        private const uint WM_HOTKEY = 0x0312;
        private const ModifierKeys HotKeyModifiers = ModifierKeys.Control | ModifierKeys.Shift | ModifierKeys.Alt;

        private readonly IWindowMessageMonitor _windowMessageMonitor;
        private readonly IHotKeysInterops _hotKeysInterops;
        private IntPtr? _hwnd;
        private HashSet<HotKeyType> _registeredHotKeys = [];

        public HotKeysService(IWindowMessageMonitor windowMessageMonitor, IHotKeysInterops hotKeysInterops)
        {
            _windowMessageMonitor = windowMessageMonitor;
            _hotKeysInterops = hotKeysInterops;
        }

        public void RegisterHotKeys(IntPtr hwnd)
        {
            if (_hwnd.HasValue)
                throw new InvalidOperationException($"This service can only be registered to one window at a time.");

            _hwnd = hwnd;

            RegisterHotKey(_hwnd.Value, HotKeyType.StartStop, HotKeyModifiers, (uint)VirtualKey.F);
            RegisterHotKey(_hwnd.Value, HotKeyType.ZoomIn, HotKeyModifiers, (uint)AdditionalKeys.Plus);
            RegisterHotKey(_hwnd.Value, HotKeyType.ZoomOut, HotKeyModifiers, (uint)AdditionalKeys.Minus);

            _windowMessageMonitor.Init(hwnd);
            _windowMessageMonitor.WindowMessageReceived += WindowMessageMonitor_WindowMessageReceived;
        }

        public void Unregister()
        {
            if (_hwnd.HasValue)
            {
                foreach (var hotKeyType in _registeredHotKeys)
                {
                    _hotKeysInterops.UnregisterHotKey(_hwnd.Value, (int)hotKeyType);
                }
            }

            _hwnd = null;
            _windowMessageMonitor.Reset();
        }

        public void Dispose()
        {
            Unregister();
            _windowMessageMonitor.WindowMessageReceived -= WindowMessageMonitor_WindowMessageReceived;
        }

        private void RegisterHotKey(IntPtr _hwnd, HotKeyType hotKeyType, ModifierKeys modifiers, uint key)
        {
            if (_hotKeysInterops.RegisterHotKey(_hwnd, (int)hotKeyType, (uint)modifiers, key))
                _registeredHotKeys.Add(hotKeyType);
            else
                OnHotKeyRegistrationFailed(new(hotKeyType));
        }

        private void OnHotKeyInvoked(HotKeyInvokedEventArgs e)
        {
            var raiseEvent = HotKeyInvoked;
            raiseEvent?.Invoke(this, e);
        }

        private void OnHotKeyRegistrationFailed(HotKeyRegistrationFailedEventArgs e)
        {
            var raiseEvent = HotKeyRegistrationFailed;
            raiseEvent?.Invoke(this, e);
        }

        private void WindowMessageMonitor_WindowMessageReceived(object? sender, WindowMessageEventArgs e)
        {
            if (e.MessageId != WM_HOTKEY)
                return;

            OnHotKeyInvoked(new((HotKeyType)e.WParam));
        }
    }
}
