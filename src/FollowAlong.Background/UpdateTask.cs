using Microsoft.Extensions.DependencyInjection;
using MrCapitalQ.FollowAlong.Core;
using MrCapitalQ.FollowAlong.Core.AppData;
using MrCapitalQ.FollowAlong.Core.Startup;
using MrCapitalQ.FollowAlong.Infrastructure;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Background;

namespace MrCapitalQ.FollowAlong.Background;

public sealed class UpdateTask : IBackgroundTask
{
    public async void Run(IBackgroundTaskInstance taskInstance)
    {
        var deferral = taskInstance.GetDeferral();
        try
        {
            var services = new ServiceCollection()
                .AddStartupTaskService()
                .AddLocalApplicationDataStore()
                .AddSettingsService()
                .BuildServiceProvider();

            var settingsService = services.GetRequiredService<ISettingsService>();
            var startupState = await services.GetRequiredService<IStartupTaskService>().GetStartupStateAsync();
            if (startupState is AppStartupState.Enabled or AppStartupState.EnabledByPolicy
                && settingsService.GetHasBeenLaunchedOnce())
                await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppWithArgumentsAsync("-silent");
        }
        finally
        {
            deferral.Complete();
        }
    }
}
