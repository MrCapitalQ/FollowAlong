using MrCapitalQ.FollowAlong.Core.Keyboard;
using MrCapitalQ.FollowAlong.ViewModels;

namespace MrCapitalQ.FollowAlong.Tests.ViewModels;

public class ShortcutKeysViewModelTests
{
    [Fact]
    public void Ctor_InitializesDisplayKeysWithModifiers()
    {
        var expected = new List<string> { "Win", "Ctrl", "Shift", "Alt", "A" };

        var viewModel = new ShortcutKeysViewModel(new(ModifierKeys.WinKey | ModifierKeys.Control | ModifierKeys.Shift | ModifierKeys.Alt, PrimaryShortcutKey.A));

        Assert.Equivalent(expected, viewModel.DisplayKeys);
    }

    [InlineData(PrimaryShortcutKey.A, "A")]
    [InlineData(PrimaryShortcutKey.Number0, "0")]
    [InlineData(PrimaryShortcutKey.NumberPad0, "0")]
    [InlineData(PrimaryShortcutKey.Number1, "1")]
    [InlineData(PrimaryShortcutKey.NumberPad1, "1")]
    [InlineData(PrimaryShortcutKey.Number2, "2")]
    [InlineData(PrimaryShortcutKey.NumberPad2, "2")]
    [InlineData(PrimaryShortcutKey.Number3, "3")]
    [InlineData(PrimaryShortcutKey.NumberPad3, "3")]
    [InlineData(PrimaryShortcutKey.Number4, "4")]
    [InlineData(PrimaryShortcutKey.NumberPad4, "4")]
    [InlineData(PrimaryShortcutKey.Number5, "5")]
    [InlineData(PrimaryShortcutKey.NumberPad5, "5")]
    [InlineData(PrimaryShortcutKey.Number6, "6")]
    [InlineData(PrimaryShortcutKey.NumberPad6, "6")]
    [InlineData(PrimaryShortcutKey.Number7, "7")]
    [InlineData(PrimaryShortcutKey.NumberPad7, "7")]
    [InlineData(PrimaryShortcutKey.Number8, "8")]
    [InlineData(PrimaryShortcutKey.NumberPad8, "8")]
    [InlineData(PrimaryShortcutKey.Number9, "9")]
    [InlineData(PrimaryShortcutKey.NumberPad9, "9")]
    [InlineData(PrimaryShortcutKey.Plus, "+")]
    [InlineData(PrimaryShortcutKey.Minus, "-")]
    [Theory]
    public void Ctor_InitializesDisplayKeysWithPrimaryShortcutKey(PrimaryShortcutKey key, string expectedDisplayKey)
    {
        var expected = new List<string> { expectedDisplayKey };

        var viewModel = new ShortcutKeysViewModel(new(ModifierKeys.None, key));

        Assert.Equivalent(expected, viewModel.DisplayKeys);
    }
}
