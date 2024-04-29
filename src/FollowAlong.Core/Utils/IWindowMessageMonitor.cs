namespace MrCapitalQ.FollowAlong.Core.Utils;

public interface IWindowMessageMonitor
{
    event EventHandler<WindowMessageEventArgs>? WindowMessageReceived;
    void Init(IntPtr hwnd);
    void Reset();
}
