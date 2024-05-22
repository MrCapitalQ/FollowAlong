using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MrCapitalQ.FollowAlong.Core.AppData;
using MrCapitalQ.FollowAlong.Infrastructure.Startup;
using System.Diagnostics.CodeAnalysis;

namespace MrCapitalQ.FollowAlong.Infrastructure;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStartupTaskService(this IServiceCollection services)
    {
        services.TryAddTransient<IStartupTaskService, StartupTaskService>();
        return services;
    }

    public static IServiceCollection AddLocalApplicationDataStore(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.TryAddSingleton<IApplicationDataStore, LocalApplicationDataStore>();
        return services;
    }
}
