namespace MrCapitalQ.FollowAlong.Infrastructure.HotKeys;

public interface IHotKeysService
{
    event EventHandler<HotKeyInvokedEventArgs>? HotKeyInvoked;

    IReadOnlyCollection<HotKeyType> HotKeyRegistrationFailures { get; }

    void RegisterHotKeys(nint hwnd);
    void Unregister();
}