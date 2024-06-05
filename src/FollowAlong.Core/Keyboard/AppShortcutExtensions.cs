using MrCapitalQ.FollowAlong.Core.AppData;

namespace MrCapitalQ.FollowAlong.Core.Keyboard;

public static class AppShortcutExtensions
{
    private const ModifierKeys DefaultModifierKeys = ModifierKeys.Control | ModifierKeys.Shift | ModifierKeys.Alt;
    private static readonly Dictionary<AppShortcutKind, ShortcutKeys> s_shortcutKeysDefaults = new()
    {
        { AppShortcutKind.StartStop, new(DefaultModifierKeys, PrimaryShortcutKey.F) },
        { AppShortcutKind.ToggleTracking, new(DefaultModifierKeys, PrimaryShortcutKey.P) },
        { AppShortcutKind.ZoomIn, new(DefaultModifierKeys, PrimaryShortcutKey.Plus) },
        { AppShortcutKind.ZoomOut, new(DefaultModifierKeys, PrimaryShortcutKey.Minus) },
        { AppShortcutKind.ResetZoom, new(DefaultModifierKeys, PrimaryShortcutKey.Number0) },
    };

    public static ShortcutKeys GetDefaultShortcutKeys(this AppShortcutKind shortcutKind) =>
        s_shortcutKeysDefaults.TryGetValue(shortcutKind, out var shortcutKeys)
            ? shortcutKeys
            : ShortcutKeys.None;
}
