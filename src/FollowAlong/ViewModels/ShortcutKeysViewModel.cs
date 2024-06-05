using MrCapitalQ.FollowAlong.Core.AppData;
using MrCapitalQ.FollowAlong.Core.Keyboard;

namespace MrCapitalQ.FollowAlong.ViewModels;

internal record ShortcutKeysViewModel
{
    public ShortcutKeysViewModel(ShortcutKeys shortcutKeys)
    {
        ModifierKeys = shortcutKeys.ModifierKeys;
        Key = shortcutKeys.Key;

        var displayKeys = new List<string>();

        if (ModifierKeys.HasFlag(ModifierKeys.WinKey))
            displayKeys.Add("Win");
        if (ModifierKeys.HasFlag(ModifierKeys.Control))
            displayKeys.Add("Ctrl");
        if (ModifierKeys.HasFlag(ModifierKeys.Shift))
            displayKeys.Add("Shift");
        if (ModifierKeys.HasFlag(ModifierKeys.Alt))
            displayKeys.Add("Alt");

        var keyDisplayString = Key switch
        {
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
            PrimaryShortcutKey.Plus => "+",
            PrimaryShortcutKey.Minus => "-",
            _ => Key.ToString()
        };
        displayKeys.Add(keyDisplayString);

        DisplayKeys = displayKeys;
    }

    public ModifierKeys ModifierKeys { get; }
    public PrimaryShortcutKey Key { get; }
    public IEnumerable<string> DisplayKeys { get; }
};
