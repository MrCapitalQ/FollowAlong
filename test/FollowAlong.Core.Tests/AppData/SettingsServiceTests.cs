using MrCapitalQ.FollowAlong.Core.AppData;
using MrCapitalQ.FollowAlong.Core.Keyboard;
using System.Text.Json;

namespace MrCapitalQ.FollowAlong.Core.Tests.AppData;

public class SettingsServiceTests
{
    private const string HasBeenLaunchedOnceKey = "HasBeenLaunchedOnce";
    private const string ShortcutKeySettingsKey = "ShortcutKeys_StartStop";
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
    public void GetShortcutKeys_DataStoreReturnsNull_SetsAndReturnsValue()
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
}
