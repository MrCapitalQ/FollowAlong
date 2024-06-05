namespace MrCapitalQ.FollowAlong.Core.Utils;

public interface IWindowMessageMonitor
{
    event EventHandler<WindowMessageEventArgs>? WindowMessageReceived;
    void Init(nint hwnd);
    void Reset();
}
