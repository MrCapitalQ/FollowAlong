namespace MrCapitalQ.FollowAlong.Core.HotKeys;

public interface IHotKeysService
{
    event System.EventHandler<HotKeyInvokedEventArgs>? HotKeyInvoked;
    event System.EventHandler<HotKeyRegistrationFailedEventArgs>? HotKeyRegistrationFailed;

    void RegisterHotKeys(nint hwnd);
    void Unregister();
}