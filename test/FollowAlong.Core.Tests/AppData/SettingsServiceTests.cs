using MrCapitalQ.FollowAlong.Core.AppData;

namespace MrCapitalQ.FollowAlong.Core.Tests.AppData;

public class SettingsServiceTests
{
    private const string HasBeenLaunchedOnceKey = "HasBeenLaunchedOnce";
    private const string AppExitBehaviorKey = "AppExitBehavior";
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
}
