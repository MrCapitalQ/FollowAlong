using MrCapitalQ.FollowAlong.Core.AppData;
using MrCapitalQ.FollowAlong.Core.Keyboard;
using MrCapitalQ.FollowAlong.ViewModels;
using Windows.System;

namespace MrCapitalQ.FollowAlong.Tests.ViewModels;

public class ShortcutKeysViewModelTests
{
    [Fact]
    public void Ctor_InitializesProperties()
    {
        var expectedModifiers = ModifierKeys.WinKey | ModifierKeys.Control | ModifierKeys.Shift | ModifierKeys.Alt;
        var expectedKey = PrimaryShortcutKey.A;
        var expectedVirtualKeys = new List<VirtualKey>
        {
            VirtualKey.LeftWindows,
            VirtualKey.Control,
            VirtualKey.Shift,
            VirtualKey.Menu,
            VirtualKey.A
        };
        var shortcutKeys = new ShortcutKeys(expectedModifiers, expectedKey);

        var viewModel = new ShortcutKeysViewModel(shortcutKeys);

        Assert.Equivalent(expectedVirtualKeys, viewModel.VirtualKeys);
    }
}
