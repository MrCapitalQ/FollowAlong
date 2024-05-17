﻿using H.NotifyIcon.EfficiencyMode;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using MrCapitalQ.FollowAlong.Core.Capture;
using System.Diagnostics.CodeAnalysis;
using WinUIEx;

namespace MrCapitalQ.FollowAlong;

[ExcludeFromCodeCoverage]
public partial class App : Application
{
    private readonly IBitmapCaptureService _captureService;

    public App(IServiceProvider services, IBitmapCaptureService captureService)
    {
        InitializeComponent();

        Services = services;
        _captureService = captureService;

        AppInstance.GetCurrent().Activated += App_Activated;
    }

    public static new App Current => (App)Application.Current;
    public IServiceProvider Services { get; }
    public LifetimeWindow? LifetimeWindow { get; protected set; }
    public MainWindow? MainWindow { get; protected set; }

    public void ShowMainWindow()
    {
        if (_captureService.IsStarted)
            return;

        EfficiencyModeUtilities.SetEfficiencyMode(false);

        if (MainWindow is null)
        {
            MainWindow = Services.GetRequiredService<MainWindow>();
            MainWindow.Closed += MainWindow_Closed;
        }

        MainWindow.Activate();
        MainWindow.SetForegroundWindow();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        LifetimeWindow = Services.GetRequiredService<LifetimeWindow>();
        LifetimeWindow.Closed += LifetimeWindow_Closed;

        if (AppInstance.GetCurrent().GetActivatedEventArgs().Kind != ExtendedActivationKind.StartupTask)
            ShowMainWindow();
    }

    private void LifetimeWindow_Closed(object sender, WindowEventArgs args)
    {
        if (sender is LifetimeWindow window)
            window.Closed -= LifetimeWindow_Closed;

        MainWindow?.Close();
        LifetimeWindow = null;

        var hostApplicationLifetime = Services.GetRequiredService<IHostApplicationLifetime>();
        hostApplicationLifetime.StopApplication();
    }

    private void App_Activated(object? sender, AppActivationArguments e)
    {
        if (e.Kind != ExtendedActivationKind.Launch)
            return;

        LifetimeWindow?.DispatcherQueue.TryEnqueue(ShowMainWindow);
    }

    private void MainWindow_Closed(object sender, WindowEventArgs args)
    {
        EfficiencyModeUtilities.SetEfficiencyMode(true);

        if (sender is MainWindow window)
            window.Closed -= MainWindow_Closed;

        MainWindow = null;
    }
}
