namespace MrCapitalQ.FollowAlong.Infrastructure.Utils;

public interface IWindowMessageMonitor
{
    event EventHandler<WindowMessageEventArgs>? WindowMessageReceived;
    void Init(IntPtr hwnd);
    void Reset();
}
