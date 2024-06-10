using MrCapitalQ.FollowAlong.Core.AppData;
using MrCapitalQ.FollowAlong.Core.Keyboard;
using System.Text.Json;

namespace MrCapitalQ.FollowAlong.Core.Tests.AppData;

public class SettingsServiceTests
{
    private const string HasBeenLaunchedOnceKey = "HasBeenLaunchedOnce";
    private const string ShortcutKeySettingsKey = "ShortcutKeys_StartStop";
    private const string HorizontalBoundsThresholdSettingsKey = "HorizontalBoundsThreshold";
    private const string VerticalBoundsThresholdSettingsKey = "VerticalBoundsThreshold";
    private readonly IApplicationDataStore _applicationDataStore;

    private readonly SettingsService _settingsService;

    public SettingsServiceTests()
    {
        _applicationDataStore = Substitute.For<IApplicationDataStore>();

        _settingsService = new(_applicationDataStore);
    }

    [Fact]
    public void GetHasBeenLaunchedOnce_DataStoreReturnsBool_ReturnsValue()
    {
        _applicationDataStore.GetValue(HasBeenLaunchedOnceKey).Returns(true);

        var actual = _settingsService.GetHasBeenLaunchedOnce();

        Assert.True(actual);
        _applicationDataStore.Received(1).GetValue(HasBeenLaunchedOnceKey);
    }

    [Fact]
    public void GetHasBeenLaunchedOnce_DataStoreReturnsNull_ReturnsFalse()
    {
        _applicationDataStore.GetValue(HasBeenLaunchedOnceKey).Returns(null);

        var actual = _settingsService.GetHasBeenLaunchedOnce();

        Assert.False(actual);
        _applicationDataStore.Received(1).GetValue(HasBeenLaunchedOnceKey);
    }

    [Fact]
    public void SetHasBeenLaunchedOnce_SavesTrueValueInApplicationDataStore()
    {
        _settingsService.SetHasBeenLaunchedOnce();

        _applicationDataStore.Received(1).SetValue(HasBeenLaunchedOnceKey, true);
    }

    [Fact]
    public void GetShortcutKeys_DataStoreReturnsNonNull_ReturnsValue()
    {
        var expected = AppShortcutKind.StartStop.GetDefaultShortcutKeys();
        _applicationDataStore.GetValue(ShortcutKeySettingsKey).Returns(JsonSerializer.Serialize(expected));

        var actual = _settingsService.GetShortcutKeys(AppShortcutKind.StartStop);

        Assert.Equal(expected, actual);
        _applicationDataStore.Received(1).GetValue(ShortcutKeySettingsKey);
    }

    [Fact]
    public void GetShortcutKeys_DataStoreReturnsNull_SetsAndReturnsDefaultValue()
    {
        var expected = AppShortcutKind.StartStop.GetDefaultShortcutKeys();
        _applicationDataStore.GetValue(ShortcutKeySettingsKey).Returns(null);

        var actual = _settingsService.GetShortcutKeys(AppShortcutKind.StartStop);

        Assert.Equal(expected, actual);
        _applicationDataStore.Received(1).SetValue(ShortcutKeySettingsKey, JsonSerializer.Serialize(expected));
        _applicationDataStore.Received(1).GetValue(ShortcutKeySettingsKey);
    }

    [Fact]
    public void SetShortcutKeys_SavesValueInApplicationDataStore()
    {
        var shortcutKeys = AppShortcutKind.StartStop.GetDefaultShortcutKeys();

        _settingsService.SetShortcutKeys(AppShortcutKind.StartStop, shortcutKeys);

        _applicationDataStore.Received(1).SetValue(ShortcutKeySettingsKey, JsonSerializer.Serialize(shortcutKeys));
    }

    [Fact]
    public void GetHorizontalBoundsThreshold_DataStoreReturnsNonNull_ReturnsValue()
    {
        var expected = 0.25;
        _applicationDataStore.GetValue(HorizontalBoundsThresholdSettingsKey).Returns(expected);

        var actual = _settingsService.GetHorizontalBoundsThreshold();

        Assert.Equal(expected, actual);
        _applicationDataStore.Received(1).GetValue(HorizontalBoundsThresholdSettingsKey);
    }

    [Fact]
    public void GetHorizontalBoundsThreshold_DataStoreReturnsNull_SetsAndReturnsDefaultValue()
    {
        var expected = 0.5;
        _applicationDataStore.GetValue(HorizontalBoundsThresholdSettingsKey).Returns(null);

        var actual = _settingsService.GetHorizontalBoundsThreshold();

        Assert.Equal(expected, actual);
        _applicationDataStore.Received(1).SetValue(HorizontalBoundsThresholdSettingsKey, expected);
        _applicationDataStore.Received(1).GetValue(HorizontalBoundsThresholdSettingsKey);
    }

    [Fact]
    public void SetHorizontalBoundsThreshold_SavesValueInApplicationDataStore()
    {
        var value = 0.25;

        _settingsService.SetHorizontalBoundsThreshold(value);

        _applicationDataStore.Received(1).SetValue(HorizontalBoundsThresholdSettingsKey, value);
    }

    [Fact]
    public void GetVerticalBoundsThreshold_DataStoreReturnsNonNull_ReturnsValue()
    {
        var expected = 0.25;
        _applicationDataStore.GetValue(VerticalBoundsThresholdSettingsKey).Returns(expected);

        var actual = _settingsService.GetVerticalBoundsThreshold();

        Assert.Equal(expected, actual);
        _applicationDataStore.Received(1).GetValue(VerticalBoundsThresholdSettingsKey);
    }

    [Fact]
    public void GetVerticalBoundsThreshold_DataStoreReturnsNull_SetsAndReturnsDefaultValue()
    {
        var expected = 0.5;
        _applicationDataStore.GetValue(VerticalBoundsThresholdSettingsKey).Returns(null);

        var actual = _settingsService.GetVerticalBoundsThreshold();

        Assert.Equal(expected, actual);
        _applicationDataStore.Received(1).SetValue(VerticalBoundsThresholdSettingsKey, expected);
        _applicationDataStore.Received(1).GetValue(VerticalBoundsThresholdSettingsKey);
    }

    [Fact]
    public void SetVerticalBoundsThreshold_SavesValueInApplicationDataStore()
    {
        var value = 0.25;

        _settingsService.SetVerticalBoundsThreshold(value);

        _applicationDataStore.Received(1).SetValue(VerticalBoundsThresholdSettingsKey, value);
    }
}
