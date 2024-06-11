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

    public static string ToDisplayString(this ShortcutKeys shortcutKeys)
    {
        var keys = new List<string>();

        if (shortcutKeys.ModifierKeys.HasFlag(ModifierKeys.WinKey))
            keys.Add("Win");
        if (shortcutKeys.ModifierKeys.HasFlag(ModifierKeys.Control))
            keys.Add("Ctrl");
        if (shortcutKeys.ModifierKeys.HasFlag(ModifierKeys.Shift))
            keys.Add("Shift");
        if (shortcutKeys.ModifierKeys.HasFlag(ModifierKeys.Alt))
            keys.Add("Alt");

        if (shortcutKeys.Key is not PrimaryShortcutKey.None)
        {
            var keyString = shortcutKeys.Key switch
            {
                PrimaryShortcutKey.Back => "Backspace",
                PrimaryShortcutKey.CapitalLock => "Caps",
                PrimaryShortcutKey.Escape => "Esc",
                PrimaryShortcutKey.PageUp => "PgUp",
                PrimaryShortcutKey.PageDown => "PgDn",
                PrimaryShortcutKey.Delete => "Del",
                PrimaryShortcutKey.Number0 or PrimaryShortcutKey.NumberPad0 => "0",
                PrimaryShortcutKey.Number1 or PrimaryShortcutKey.NumberPad1 => "1",
                PrimaryShortcutKey.Number2 or PrimaryShortcutKey.NumberPad2 => "2",
                PrimaryShortcutKey.Number3 or PrimaryShortcutKey.NumberPad3 => "3",
                PrimaryShortcutKey.Number4 or PrimaryShortcutKey.NumberPad4 => "4",
                PrimaryShortcutKey.Number5 or PrimaryShortcutKey.NumberPad5 => "5",
                PrimaryShortcutKey.Number6 or PrimaryShortcutKey.NumberPad6 => "6",
                PrimaryShortcutKey.Number7 or PrimaryShortcutKey.NumberPad7 => "7",
                PrimaryShortcutKey.Number8 or PrimaryShortcutKey.NumberPad8 => "8",
                PrimaryShortcutKey.Number9 or PrimaryShortcutKey.NumberPad9 => "9",
                PrimaryShortcutKey.SemiColon => ";",
                PrimaryShortcutKey.Comma => ",",
                PrimaryShortcutKey.Period => ".",
                PrimaryShortcutKey.ForwardSlash => "/",
                PrimaryShortcutKey.Backtick => "`",
                PrimaryShortcutKey.LeftSquareBracket => "[",
                PrimaryShortcutKey.BackSlash => "\\",
                PrimaryShortcutKey.RightSquareBracket => "]",
                PrimaryShortcutKey.SingleQuote => "'",
                _ => shortcutKeys.Key.ToString()
            };
            keys.Add(keyString);
        }

        return string.Join(" + ", keys);
    }
}
