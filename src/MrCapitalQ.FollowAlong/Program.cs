using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MrCapitalQ.FollowAlong;

Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<WinUIHostedService<App>>();
        services.AddSingleton<App>();
        services.AddTransient<MainWindow>();
    })
    .Build().Run();
