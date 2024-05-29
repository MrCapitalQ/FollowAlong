using MrCapitalQ.FollowAlong.Infrastructure.Utils;

namespace MrCapitalQ.FollowAlong.Infrastructure.Display;

public sealed class DisplayWatcher : IDisposable
{
    public event EventHandler? DisplayChanged;

    private const uint WM_DISPLAYCHANGE = 0x07E;

    private readonly IWindowMessageMonitor _windowMessageMonitor;
    private IntPtr? _hwnd;

    public DisplayWatcher(IWindowMessageMonitor windowMessageMonitor)
    {
        _windowMessageMonitor = windowMessageMonitor;
    }

    public void Register(IntPtr hwnd)
    {
        if (_hwnd.HasValue)
            throw new InvalidOperationException("This service can only be registered to one window at a time.");

        _hwnd = hwnd;

        _windowMessageMonitor.Init(_hwnd.Value);
        _windowMessageMonitor.WindowMessageReceived += WindowMessageMonitor_WindowMessageReceived;
    }

    public void Unregister()
    {
        _hwnd = null;
        _windowMessageMonitor.Reset();
    }

    public void Dispose()
    {
        Unregister();
        _windowMessageMonitor.WindowMessageReceived -= WindowMessageMonitor_WindowMessageReceived;
    }

    private void OnDisplayChanged()
    {
        var raiseEvent = DisplayChanged;
        raiseEvent?.Invoke(this, new());
    }

    private void WindowMessageMonitor_WindowMessageReceived(object? sender, WindowMessageEventArgs e)
    {
        if (e.MessageId != WM_DISPLAYCHANGE)
            return;

        OnDisplayChanged();
    }
}
