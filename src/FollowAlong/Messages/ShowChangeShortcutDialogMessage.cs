using MrCapitalQ.FollowAlong.Core.AppData;
using MrCapitalQ.FollowAlong.Core.Keyboard;

namespace MrCapitalQ.FollowAlong.Messages;

internal record ShowChangeShortcutDialogMessage(AppShortcutKind ShortcutKind, ShortcutKeys CurrentShortcutKeys);
