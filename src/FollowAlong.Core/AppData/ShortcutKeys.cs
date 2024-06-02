using MrCapitalQ.FollowAlong.Core.Keyboard;

namespace MrCapitalQ.FollowAlong.Core.AppData;

public record ShortcutKeys(ModifierKeys ModifierKeys, PrimaryShortcutKey Key)
{
    public static ShortcutKeys None { get; } = new(ModifierKeys.None, PrimaryShortcutKey.None);
}
