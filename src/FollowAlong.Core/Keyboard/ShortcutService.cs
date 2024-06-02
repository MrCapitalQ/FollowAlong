using MrCapitalQ.FollowAlong.Core.AppData;
using MrCapitalQ.FollowAlong.Core.Utils;

namespace MrCapitalQ.FollowAlong.Core.Keyboard;

public sealed class ShortcutService(IWindowMessageMonitor windowMessageMonitor, IHotKeysInterops hotKeysInterops, ISettingsService settingsService)
    : IShortcutService, IDisposable
{
    public event EventHandler<AppShortcutInvokedEventArgs>? ShortcutInvoked;

    private const uint WM_HOTKEY = 0x0312;

    private readonly IWindowMessageMonitor _windowMessageMonitor = windowMessageMonitor;
    private readonly IHotKeysInterops _hotKeysInterops = hotKeysInterops;
    private readonly ISettingsService _settingsService = settingsService;
    private readonly HashSet<AppShortcutKind> _registeredShortcuts = [];
    private readonly HashSet<AppShortcutKind> _shortcutRegistrationFailures = [];
    private nint? _hwnd;

    public IReadOnlyCollection<AppShortcutKind> ShortcutRegistrationFailures => [.. _shortcutRegistrationFailures];

    public void Register(nint hwnd)
    {
        if (_hwnd.HasValue)
            throw new InvalidOperationException($"This service can only be registered to one window at a time.");

        _hwnd = hwnd;

        _shortcutRegistrationFailures.Clear();

        RegisterShortcut(_hwnd.Value, AppShortcutKind.StartStop);
        RegisterShortcut(_hwnd.Value, AppShortcutKind.ZoomIn);
        RegisterShortcut(_hwnd.Value, AppShortcutKind.ZoomOut);
        RegisterShortcut(_hwnd.Value, AppShortcutKind.ResetZoom);
        RegisterShortcut(_hwnd.Value, AppShortcutKind.ToggleTracking);

        _windowMessageMonitor.Init(hwnd);
        _windowMessageMonitor.WindowMessageReceived += WindowMessageMonitor_WindowMessageReceived;
    }

    public void Unregister()
    {
        if (_hwnd.HasValue)
        {
            foreach (var shortcutKind in _registeredShortcuts)
            {
                _hotKeysInterops.UnregisterHotKey(_hwnd.Value, (int)shortcutKind);
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

    private void RegisterShortcut(nint _hwnd, AppShortcutKind shortcutKind)
    {
        var shortcutKeys = _settingsService.GetShortcutKeys(shortcutKind);
        if (shortcutKeys.Key == PrimaryShortcutKey.None)
            return;

        if (_hotKeysInterops.RegisterHotKey(_hwnd, (int)shortcutKind, (uint)shortcutKeys.ModifierKeys, (uint)shortcutKeys.Key))
            _registeredShortcuts.Add(shortcutKind);
        else
            _shortcutRegistrationFailures.Add(shortcutKind);
    }

    private void OnShortcutInvoked(AppShortcutInvokedEventArgs e)
    {
        var raiseEvent = ShortcutInvoked;
        raiseEvent?.Invoke(this, e);
    }

    private void WindowMessageMonitor_WindowMessageReceived(object? sender, WindowMessageEventArgs e)
    {
        if (e.MessageId != WM_HOTKEY)
            return;

        OnShortcutInvoked(new((AppShortcutKind)e.WParam));
    }
}
