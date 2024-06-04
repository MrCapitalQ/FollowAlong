using MrCapitalQ.FollowAlong.Core.AppData;
using MrCapitalQ.FollowAlong.Core.Keyboard;
using Windows.System;

namespace MrCapitalQ.FollowAlong.Shared;

public static class ShortcutKeysExtensions
{
    public static IEnumerable<VirtualKey> ToVirtualKeys(this ShortcutKeys shortcutKeys)
    {
        var virtualKeys = new List<VirtualKey>();

        if (shortcutKeys.ModifierKeys.HasFlag(ModifierKeys.WinKey))
            virtualKeys.Add(VirtualKey.LeftWindows);
        if (shortcutKeys.ModifierKeys.HasFlag(ModifierKeys.Control))
            virtualKeys.Add(VirtualKey.Control);
        if (shortcutKeys.ModifierKeys.HasFlag(ModifierKeys.Shift))
            virtualKeys.Add(VirtualKey.Shift);
        if (shortcutKeys.ModifierKeys.HasFlag(ModifierKeys.Alt))
            virtualKeys.Add(VirtualKey.Menu);

        if (shortcutKeys.Key is not PrimaryShortcutKey.None)
            virtualKeys.Add((VirtualKey)shortcutKeys.Key);

        return virtualKeys;
    }

    public static ShortcutKeys ToShortcutKeys(this IEnumerable<VirtualKey> virtualKeys)
    {
        var modifierKeys = ModifierKeys.None;
        var primaryShortcutKey = PrimaryShortcutKey.None;
        foreach (var key in virtualKeys)
        {
            if (key is VirtualKey.LeftWindows or VirtualKey.RightWindows)
                modifierKeys |= ModifierKeys.WinKey;
            else if (key is VirtualKey.Control)
                modifierKeys |= ModifierKeys.Control;
            else if (key is VirtualKey.Shift)
                modifierKeys |= ModifierKeys.Shift;
            else if (key is VirtualKey.Menu)
                modifierKeys |= ModifierKeys.Alt;
            else
                primaryShortcutKey = (PrimaryShortcutKey)key;
        }

        return new ShortcutKeys(modifierKeys, primaryShortcutKey);
    }
}
