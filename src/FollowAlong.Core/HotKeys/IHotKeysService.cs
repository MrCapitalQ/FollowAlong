namespace MrCapitalQ.FollowAlong.Core.HotKeys;

public interface IHotKeysService
{
    event EventHandler<HotKeyInvokedEventArgs>? HotKeyInvoked;

    IReadOnlyCollection<HotKeyType> RegisteredHotKeys { get; }

    void RegisterHotKeys(nint hwnd);
    void Unregister();
}