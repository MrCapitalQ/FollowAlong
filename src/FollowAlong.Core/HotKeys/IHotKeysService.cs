namespace MrCapitalQ.FollowAlong.Core.HotKeys;

public interface IHotKeysService
{
    event EventHandler<HotKeyInvokedEventArgs>? HotKeyInvoked;

    IReadOnlyCollection<HotKeyType> HotKeyRegistrationFailures { get; }

    void RegisterHotKeys(nint hwnd);
    void Unregister();
}