namespace MrCapitalQ.FollowAlong.Core.Startup;

public interface IStartupTaskService
{
    Task<AppStartupState> GetStartupStateAsync();
    Task SetStartupStateAsync(bool isEnabled);
}
