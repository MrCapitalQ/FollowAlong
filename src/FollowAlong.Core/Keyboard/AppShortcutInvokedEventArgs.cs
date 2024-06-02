namespace MrCapitalQ.FollowAlong.Core.Keyboard;

public class AppShortcutInvokedEventArgs(AppShortcutKind shortcutKind) : EventArgs
{
    public AppShortcutKind ShortcutKind { get; } = shortcutKind;
}
