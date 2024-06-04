using MrCapitalQ.FollowAlong.Core.AppData;
using MrCapitalQ.FollowAlong.Core.Keyboard;
using MrCapitalQ.FollowAlong.Shared;
using Windows.System;

namespace MrCapitalQ.FollowAlong.Tests.Shared;

public class ShortcutKeysExtensionsTests
{
    [Fact]
    public void ToVirtualKeys_NonNonePrimaryKey_ReturnsVirtualKeys()
    {
        List<VirtualKey> expected = [VirtualKey.LeftWindows, VirtualKey.Control, VirtualKey.Shift, VirtualKey.Menu, VirtualKey.Q];
        var shortcutKeys = new ShortcutKeys(ModifierKeys.WinKey | ModifierKeys.Control | ModifierKeys.Shift | ModifierKeys.Alt, PrimaryShortcutKey.Q);

        var actual = shortcutKeys.ToVirtualKeys();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ToVirtualKeys_NonePrimaryKey_ReturnsOnlyModifierVirtualKeys()
    {
        List<VirtualKey> expected = [VirtualKey.LeftWindows, VirtualKey.Control, VirtualKey.Shift, VirtualKey.Menu];
        var shortcutKeys = new ShortcutKeys(ModifierKeys.WinKey | ModifierKeys.Control | ModifierKeys.Shift | ModifierKeys.Alt, PrimaryShortcutKey.None);

        var actual = shortcutKeys.ToVirtualKeys();

        Assert.Equal(expected, actual);
    }

    [InlineData(VirtualKey.Q, ModifierKeys.None, PrimaryShortcutKey.Q)]
    [InlineData(VirtualKey.LeftWindows, ModifierKeys.WinKey, PrimaryShortcutKey.None)]
    [InlineData(VirtualKey.RightWindows, ModifierKeys.WinKey, PrimaryShortcutKey.None)]
    [InlineData(VirtualKey.Control, ModifierKeys.Control, PrimaryShortcutKey.None)]
    [InlineData(VirtualKey.Shift, ModifierKeys.Shift, PrimaryShortcutKey.None)]
    [InlineData(VirtualKey.Menu, ModifierKeys.Alt, PrimaryShortcutKey.None)]
    [Theory]
    public void ToShortcutKeys(VirtualKey virtualKey, ModifierKeys expectedModifiers, PrimaryShortcutKey expectedKey)
    {
        var expected = new ShortcutKeys(expectedModifiers, expectedKey);

        var actual = new List<VirtualKey> { virtualKey }.ToShortcutKeys();

        Assert.Equal(expected, actual);
    }
}
