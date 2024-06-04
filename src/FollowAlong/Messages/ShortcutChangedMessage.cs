using MrCapitalQ.FollowAlong.Core.AppData;
using MrCapitalQ.FollowAlong.Core.Keyboard;

namespace MrCapitalQ.FollowAlong.Messages;

internal record ShortcutChangedMessage(AppShortcutKind ShortcutKind, ShortcutKeys ShortcutKeys);
