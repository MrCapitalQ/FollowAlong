using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace MrCapitalQ.FollowAlong;

[ExcludeFromCodeCoverage]
internal sealed class WindowsAppHostedService<TApplication> : IHostedService
    where TApplication : Application
{
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly IServiceProvider _serviceProvider;

    public WindowsAppHostedService(IHostApplicationLifetime hostApplicationLifetime, IServiceProvider serviceProvider)
    {
        _hostApplicationLifetime = hostApplicationLifetime;
        _serviceProvider = serviceProvider;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var thread = new Thread(Main);
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private void Main()
    {
        WinRT.ComWrappersSupport.InitializeComWrappers();
        Application.Start(p =>
        {
            var context = new DispatcherQueueSynchronizationContext(DispatcherQueue.GetForCurrentThread());
            SynchronizationContext.SetSynchronizationContext(context);
            _serviceProvider.GetRequiredService<TApplication>();
        });
        _hostApplicationLifetime.StopApplication();
    }
}
