namespace MrCapitalQ.FollowAlong.Infrastructure.Startup;

public interface IStartupTaskService
{
    Task<AppStartupState> GetStartupStateAsync();
    Task SetStartupStateAsync(bool isEnabled);
}
