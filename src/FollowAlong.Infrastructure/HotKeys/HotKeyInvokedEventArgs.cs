namespace MrCapitalQ.FollowAlong.Infrastructure.HotKeys;

public class HotKeyInvokedEventArgs : EventArgs
{
    public HotKeyInvokedEventArgs(HotKeyType hotKeyType) => HotKeyType = hotKeyType;

    public HotKeyType HotKeyType { get; }
}
