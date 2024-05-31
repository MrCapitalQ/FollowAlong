using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MrCapitalQ.FollowAlong.Core.AppData;
using System.Diagnostics.CodeAnalysis;

namespace MrCapitalQ.FollowAlong.Core;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSettingsService(this IServiceCollection services)
    {
        services.TryAddTransient<ISettingsService, SettingsService>();
        return services;
    }
}
