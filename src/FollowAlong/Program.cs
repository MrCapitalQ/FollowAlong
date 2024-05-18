using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Windows.AppLifecycle;
using MrCapitalQ.FollowAlong;
using MrCapitalQ.FollowAlong.Core.Capture;
using MrCapitalQ.FollowAlong.Core.Display;
using MrCapitalQ.FollowAlong.Core.HotKeys;
using MrCapitalQ.FollowAlong.Core.Startup;
using MrCapitalQ.FollowAlong.Core.Tracking;
using MrCapitalQ.FollowAlong.Core.Utils;
using MrCapitalQ.FollowAlong.Services;
using MrCapitalQ.FollowAlong.Shared;
using MrCapitalQ.FollowAlong.ViewModels;
using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
internal class Program
{
    private static async Task Main(string[] args)
    {
        var keyInstance = AppInstance.FindOrRegisterForKey("f3c5d6b4-6a0e-4e7c-9c1d-2a4d8d8a3d4e");
        if (!keyInstance.IsCurrent)
        {
            var activationArgs = AppInstance.GetCurrent().GetActivatedEventArgs();
            await keyInstance.RedirectActivationToAsync(activationArgs);
            return;
        }

        var builder = Host.CreateApplicationBuilder(args);

        builder.Services.AddHostedService<WindowsAppHostedService<App>>();

        builder.Services.AddSingleton<App>();
        builder.Services.AddSingleton<LifetimeWindow>();
        builder.Services.AddTransient<MainWindow>();
        builder.Services.AddTransient<PreviewWindow>();
        builder.Services.AddTransient<ShareWindow>();

        builder.Services.AddTransient<MainViewModel>();
        builder.Services.AddSingleton<PreviewViewModel>();
        builder.Services.AddSingleton<SettingsViewModel>();

        builder.Services.AddTransient<IDisplayService, DisplayService>();
        builder.Services.AddSingleton<ICaptureSessionAdapter, CaptureSessionAdapter>();
        builder.Services.AddSingleton<IBitmapCaptureService, BitmapCaptureService>();
        builder.Services.AddTransient<IPointerService, PointerService>();
        builder.Services.AddSingleton<IUpdateSynchronizer, UpdateSynchronizer>();
        builder.Services.AddTransient<TrackingTransformService>();
        builder.Services.AddSingleton<IHotKeysService, HotKeysService>();
        builder.Services.AddSingleton<DisplayWatcher>();
        builder.Services.AddTransient<IWindowMessageMonitor, WindowMessageMonitorAdapter>();
        builder.Services.AddTransient<IHotKeysInterops, HotKeysInteropsAdapter>();
        builder.Services.AddTransient<IScreenshotService, ScreenshotService>();
        builder.Services.AddTransient<IGraphicsCreator, GraphicsCreator>();
        builder.Services.AddTransient<IDisplayCaptureItemCreator, DisplayCaptureItemCreator>();
        builder.Services.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);
        builder.Services.AddSingleton<IStartupTaskService, StartupTaskService>();
        builder.Services.AddSingleton<IPackageInfo, PackageInfo>();

        var host = builder.Build();
        host.Run();
    }
}