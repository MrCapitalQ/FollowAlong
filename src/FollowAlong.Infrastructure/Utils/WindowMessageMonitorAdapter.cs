using System.Diagnostics.CodeAnalysis;

namespace MrCapitalQ.FollowAlong.Infrastructure.Utils;

[ExcludeFromCodeCoverage(Justification = "Adapter class for WindowMessageMonitor class that can't be mocked.")]
public sealed class WindowMessageMonitorAdapter : IWindowMessageMonitor
{
    public event EventHandler<WindowMessageEventArgs>? WindowMessageReceived;

    private WinUIEx.Messaging.WindowMessageMonitor? _monitor;

    public void Init(IntPtr hwnd)
    {
        _monitor = new WinUIEx.Messaging.WindowMessageMonitor(hwnd);
        _monitor.WindowMessageReceived += Monitor_WindowMessageReceived;
    }

    public void Reset()
    {
        if (_monitor is null)
            return;

        _monitor.WindowMessageReceived -= Monitor_WindowMessageReceived;
        _monitor.Dispose();
    }

    private void Monitor_WindowMessageReceived(object? sender, WinUIEx.Messaging.WindowMessageEventArgs e)
    {
        var raiseEvent = WindowMessageReceived;
        raiseEvent?.Invoke(this, new(e.Message.Hwnd, e.Message.MessageId, e.Message.WParam, e.Message.LParam));
    }
}
