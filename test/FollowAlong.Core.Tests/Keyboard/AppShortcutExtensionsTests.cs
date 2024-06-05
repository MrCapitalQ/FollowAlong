using MrCapitalQ.FollowAlong.Core.AppData;
using MrCapitalQ.FollowAlong.Core.Keyboard;

namespace MrCapitalQ.FollowAlong.Core.Tests.Keyboard;

public class AppShortcutExtensionsTests
{
    [InlineData(AppShortcutKind.StartStop, ModifierKeys.Control | ModifierKeys.Shift | ModifierKeys.Alt, PrimaryShortcutKey.F)]
    [InlineData(AppShortcutKind.ZoomIn, ModifierKeys.Control | ModifierKeys.Shift | ModifierKeys.Alt, PrimaryShortcutKey.Plus)]
    [InlineData(AppShortcutKind.ZoomOut, ModifierKeys.Control | ModifierKeys.Shift | ModifierKeys.Alt, PrimaryShortcutKey.Minus)]
    [InlineData(AppShortcutKind.ResetZoom, ModifierKeys.Control | ModifierKeys.Shift | ModifierKeys.Alt, PrimaryShortcutKey.Number0)]
    [InlineData(AppShortcutKind.ToggleTracking, ModifierKeys.Control | ModifierKeys.Shift | ModifierKeys.Alt, PrimaryShortcutKey.P)]
    [InlineData((AppShortcutKind)1000, ModifierKeys.None, PrimaryShortcutKey.None)]
    [Theory]
    public void GetDefaultShortcutKeys(AppShortcutKind shortcutKind, ModifierKeys modifierKeys, PrimaryShortcutKey key)
    {
        var expected = new ShortcutKeys(modifierKeys, key);

        var actual = shortcutKind.GetDefaultShortcutKeys();

        Assert.Equal(expected, actual);
    }
}
