namespace MrCapitalQ.FollowAlong.Core.AppData;

public class SettingsService : ISettingsService
{
    private const string HasBeenLaunchedOnceSettingsKey = "HasBeenLaunchedOnce";
    private const string AppExitBehaviorSettingsKey = "AppExitBehavior";

    private readonly IApplicationDataStore _applicationDataStore;

    public SettingsService(IApplicationDataStore localApplicationData) => _applicationDataStore = localApplicationData;

    public bool GetHasBeenLaunchedOnce()
        => _applicationDataStore.GetValue(HasBeenLaunchedOnceSettingsKey) is bool value && value;

    public void SetHasBeenLaunchedOnce()
        => _applicationDataStore.SetValue(HasBeenLaunchedOnceSettingsKey, true);
}
