using MrCapitalQ.FollowAlong.Core.AppData;
using MrCapitalQ.FollowAlong.Core.Keyboard;
using Windows.System;

namespace MrCapitalQ.FollowAlong.ViewModels;

internal record ShortcutKeysViewModel
{
    public ShortcutKeysViewModel(ShortcutKeys shortcutKeys)
    {
        ModifierKeys = shortcutKeys.ModifierKeys;
        Key = shortcutKeys.Key;

        var virtualKeys = new List<VirtualKey>();

        if (ModifierKeys.HasFlag(ModifierKeys.WinKey))
            virtualKeys.Add(VirtualKey.LeftWindows);
        if (ModifierKeys.HasFlag(ModifierKeys.Control))
            virtualKeys.Add(VirtualKey.Control);
        if (ModifierKeys.HasFlag(ModifierKeys.Shift))
            virtualKeys.Add(VirtualKey.Shift);
        if (ModifierKeys.HasFlag(ModifierKeys.Alt))
            virtualKeys.Add(VirtualKey.Menu);

        virtualKeys.Add((VirtualKey)Key);

        VirtualKeys = virtualKeys;
    }

    public ModifierKeys ModifierKeys { get; }
    public PrimaryShortcutKey Key { get; }
    public IEnumerable<VirtualKey> VirtualKeys { get; }
};
