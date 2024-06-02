namespace MrCapitalQ.FollowAlong.Core.Keyboard;

public interface IShortcutService
{
    event EventHandler<AppShortcutInvokedEventArgs>? ShortcutInvoked;

    IReadOnlyCollection<AppShortcutKind> ShortcutRegistrationFailures { get; }

    void Register(nint hwnd);
    void Unregister();
}