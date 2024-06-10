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

    [InlineData(ModifierKeys.None, PrimaryShortcutKey.None, "")]
    [InlineData(ModifierKeys.WinKey | ModifierKeys.Control | ModifierKeys.Shift | ModifierKeys.Alt, PrimaryShortcutKey.Q, "Win + Ctrl + Shift + Alt + Q")]
    [InlineData(ModifierKeys.None, PrimaryShortcutKey.Back, "Backspace")]
    [InlineData(ModifierKeys.None, PrimaryShortcutKey.CapitalLock, "Caps")]
    [InlineData(ModifierKeys.None, PrimaryShortcutKey.Escape, "Esc")]
    [InlineData(ModifierKeys.None, PrimaryShortcutKey.PageUp, "PgUp")]
    [InlineData(ModifierKeys.None, PrimaryShortcutKey.PageDown, "PgDn")]
    [InlineData(ModifierKeys.None, PrimaryShortcutKey.Delete, "Del")]
    [InlineData(ModifierKeys.None, PrimaryShortcutKey.Number0, "0")]
    [InlineData(ModifierKeys.None, PrimaryShortcutKey.Number1, "1")]
    [InlineData(ModifierKeys.None, PrimaryShortcutKey.Number2, "2")]
    [InlineData(ModifierKeys.None, PrimaryShortcutKey.Number3, "3")]
    [InlineData(ModifierKeys.None, PrimaryShortcutKey.Number4, "4")]
    [InlineData(ModifierKeys.None, PrimaryShortcutKey.Number5, "5")]
    [InlineData(ModifierKeys.None, PrimaryShortcutKey.Number6, "6")]
    [InlineData(ModifierKeys.None, PrimaryShortcutKey.Number7, "7")]
    [InlineData(ModifierKeys.None, PrimaryShortcutKey.Number8, "8")]
    [InlineData(ModifierKeys.None, PrimaryShortcutKey.Number9, "9")]
    [InlineData(ModifierKeys.None, PrimaryShortcutKey.NumberPad0, "0")]
    [InlineData(ModifierKeys.None, PrimaryShortcutKey.NumberPad1, "1")]
    [InlineData(ModifierKeys.None, PrimaryShortcutKey.NumberPad2, "2")]
    [InlineData(ModifierKeys.None, PrimaryShortcutKey.NumberPad3, "3")]
    [InlineData(ModifierKeys.None, PrimaryShortcutKey.NumberPad4, "4")]
    [InlineData(ModifierKeys.None, PrimaryShortcutKey.NumberPad5, "5")]
    [InlineData(ModifierKeys.None, PrimaryShortcutKey.NumberPad6, "6")]
    [InlineData(ModifierKeys.None, PrimaryShortcutKey.NumberPad7, "7")]
    [InlineData(ModifierKeys.None, PrimaryShortcutKey.NumberPad8, "8")]
    [InlineData(ModifierKeys.None, PrimaryShortcutKey.NumberPad9, "9")]
    [InlineData(ModifierKeys.None, PrimaryShortcutKey.SemiColon, ";")]
    [InlineData(ModifierKeys.None, PrimaryShortcutKey.Comma, ",")]
    [InlineData(ModifierKeys.None, PrimaryShortcutKey.Period, ".")]
    [InlineData(ModifierKeys.None, PrimaryShortcutKey.ForwardSlash, "/")]
    [InlineData(ModifierKeys.None, PrimaryShortcutKey.Backtick, "`")]
    [InlineData(ModifierKeys.None, PrimaryShortcutKey.LeftSquareBracket, "[")]
    [InlineData(ModifierKeys.None, PrimaryShortcutKey.BackSlash, "\\")]
    [InlineData(ModifierKeys.None, PrimaryShortcutKey.RightSquareBracket, "]")]
    [InlineData(ModifierKeys.None, PrimaryShortcutKey.SingleQuote, "'")]
    [Theory]
    public void ToDisplayString(ModifierKeys modifiers, PrimaryShortcutKey key, string expected)
    {
        var actual = new ShortcutKeys(modifiers, key).ToDisplayString();

        Assert.Equal(expected, actual);
    }
}
