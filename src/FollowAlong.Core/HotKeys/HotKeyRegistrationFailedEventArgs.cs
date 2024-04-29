namespace MrCapitalQ.FollowAlong.Core.HotKeys;

public class HotKeyRegistrationFailedEventArgs : EventArgs
{
    public HotKeyRegistrationFailedEventArgs(HotKeyType hotKeyType) => HotKeyType = hotKeyType;

    public HotKeyType HotKeyType { get; }
}
