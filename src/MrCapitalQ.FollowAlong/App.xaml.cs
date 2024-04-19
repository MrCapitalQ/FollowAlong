using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using WinUIEx;

namespace MrCapitalQ.FollowAlong;

public partial class App : Application
{
    public App(IServiceProvider services)
    {
        InitializeComponent();
        Services = services;

        AppInstance.GetCurrent().Activated += App_Activated;
    }

    public static new App Current => (App)Application.Current;
    public IServiceProvider Services { get; }
    public Window? Window { get; protected set; }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        Window = Services.GetRequiredService<MainWindow>();
        Window.Activate();
        Window.Closed += Window_Closed;
    }

    private void Window_Closed(object sender, WindowEventArgs args)
    {
        if (sender is Window window)
            window.Closed -= Window_Closed;

        Window = null;

        var hostApplicationLifetime = Services.GetRequiredService<IHostApplicationLifetime>();
        hostApplicationLifetime.StopApplication();
    }

    private void App_Activated(object? sender, AppActivationArguments e)
    {
        if (e.Kind == ExtendedActivationKind.Launch)
            Window?.DispatcherQueue.TryEnqueue(() =>
            {
                Window.Activate();
                Window.SetForegroundWindow();
            });
    }
}
