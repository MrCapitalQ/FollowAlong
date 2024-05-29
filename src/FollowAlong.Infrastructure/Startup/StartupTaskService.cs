using System.Diagnostics.CodeAnalysis;
using Windows.ApplicationModel;

namespace MrCapitalQ.FollowAlong.Infrastructure.Startup;

[ExcludeFromCodeCoverage(Justification = "Adapter class for native StartupTask class.")]
public class StartupTaskService : IStartupTaskService
{
    private const string StartupTaskId = "9e0fc765-860d-4732-80ac-673ff884ece4";

    public async Task<AppStartupState> GetStartupStateAsync()
    {
        var startupTask = await StartupTask.GetAsync(StartupTaskId);
        return (AppStartupState)startupTask.State;
    }

    public async Task SetStartupStateAsync(bool isEnabled)
    {
        var startupTask = await StartupTask.GetAsync(StartupTaskId);
        if (isEnabled)
            await startupTask.RequestEnableAsync();
        else
            startupTask.Disable();
    }
}
