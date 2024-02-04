﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Windows.AppLifecycle;
using MrCapitalQ.FollowAlong;
using MrCapitalQ.FollowAlong.Core.Capture;
using MrCapitalQ.FollowAlong.Core.Display;
using MrCapitalQ.FollowAlong.Core.HotKeys;
using MrCapitalQ.FollowAlong.Core.Tracking;
using MrCapitalQ.FollowAlong.Pages;
using MrCapitalQ.FollowAlong.ViewModels;
using System;

var keyInstance = AppInstance.FindOrRegisterForKey("f3c5d6b4-6a0e-4e7c-9c1d-2a4d8d8a3d4e");
if (!keyInstance.IsCurrent)
{
    var activationArgs = AppInstance.GetCurrent().GetActivatedEventArgs();
    await keyInstance.RedirectActivationToAsync(activationArgs);
    return;
}

Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<WinUIHostedService<App>>();
        services.AddSingleton<App>();
        services.AddSingleton<MainWindow>();
        services.AddTransient<ShareWindow>();
        services.AddTransient<MainPage>();

        services.AddSingleton<MainViewModel>();

        services.AddTransient<DisplayService>();
        services.AddSingleton<BitmapCaptureService>();
        services.AddTransient<PointerService>();
        services.AddTransient<TrackingTransformService>();
        services.AddSingleton<HotKeysService>();
        services.AddSingleton<DisplayWatcher>();
    })
    .Build()
    .Run();
