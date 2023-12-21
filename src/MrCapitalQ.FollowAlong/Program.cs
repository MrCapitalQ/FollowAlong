using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MrCapitalQ.FollowAlong;
using MrCapitalQ.FollowAlong.Core.Capture;
using MrCapitalQ.FollowAlong.Core.Monitors;
using MrCapitalQ.FollowAlong.Core.Tracking;

Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<WinUIHostedService<App>>();
        services.AddSingleton<App>();
        services.AddSingleton<MainWindow>();

        services.AddTransient<MonitorService>();
        services.AddSingleton<BitmapCaptureService>();
        services.AddTransient<PointerService>();
        services.AddTransient<TrackingTransformService>();
    })
    .Build().Run();
