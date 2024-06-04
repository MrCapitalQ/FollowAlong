using MrCapitalQ.FollowAlong.Core.AppData;
using MrCapitalQ.FollowAlong.Shared;
using Windows.System;

namespace MrCapitalQ.FollowAlong.ViewModels;

internal record ShortcutKeysViewModel
{
    public ShortcutKeysViewModel(ShortcutKeys shortcutKeys) => VirtualKeys = shortcutKeys.ToVirtualKeys();

    public IEnumerable<VirtualKey> VirtualKeys { get; }
};
