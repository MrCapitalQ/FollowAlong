using MrCapitalQ.FollowAlong.Core.Utils;
using Windows.System;

namespace MrCapitalQ.FollowAlong.Core.HotKeys;

public sealed class HotKeysService(IWindowMessageMonitor windowMessageMonitor, IHotKeysInterops hotKeysInterops)
    : IHotKeysService, IDisposable
{
    public event EventHandler<HotKeyInvokedEventArgs>? HotKeyInvoked;

    private const uint WM_HOTKEY = 0x0312;
    private const ModifierKeys HotKeyModifiers = ModifierKeys.Control | ModifierKeys.Shift | ModifierKeys.Alt;

    private readonly IWindowMessageMonitor _windowMessageMonitor = windowMessageMonitor;
    private readonly IHotKeysInterops _hotKeysInterops = hotKeysInterops;
    private readonly HashSet<HotKeyType> _registeredHotKeys = [];
    private nint? _hwnd;

    public IReadOnlyCollection<HotKeyType> RegisteredHotKeys => [.. _registeredHotKeys];

    public void RegisterHotKeys(nint hwnd)
    {
        if (_hwnd.HasValue)
            throw new InvalidOperationException($"This service can only be registered to one window at a time.");

        _hwnd = hwnd;

        RegisterHotKey(_hwnd.Value, HotKeyType.StartStop, HotKeyModifiers, (uint)VirtualKey.F);
        RegisterHotKey(_hwnd.Value, HotKeyType.ZoomIn, HotKeyModifiers, (uint)AdditionalKeys.Plus);
        RegisterHotKey(_hwnd.Value, HotKeyType.ZoomOut, HotKeyModifiers, (uint)AdditionalKeys.Minus);
        RegisterHotKey(_hwnd.Value, HotKeyType.ToggleTracking, HotKeyModifiers, (uint)VirtualKey.P);

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

    private void RegisterHotKey(nint _hwnd, HotKeyType hotKeyType, ModifierKeys modifiers, uint key)
    {
        if (_hotKeysInterops.RegisterHotKey(_hwnd, (int)hotKeyType, (uint)modifiers, key))
            _registeredHotKeys.Add(hotKeyType);
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
}
