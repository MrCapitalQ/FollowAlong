namespace MrCapitalQ.FollowAlong.Core.AppData;

public interface ISettingsService
{
    bool GetHasBeenLaunchedOnce();
    void SetHasBeenLaunchedOnce();
}