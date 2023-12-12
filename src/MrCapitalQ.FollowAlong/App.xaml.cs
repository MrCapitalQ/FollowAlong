using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using System;

namespace MrCapitalQ.FollowAlong
{
    public partial class App : Application
    {
        private readonly IServiceProvider _services;
        private Window? _window;

        public App(IServiceProvider services)
        {
            InitializeComponent();
            _services = services;
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            _window = _services.GetRequiredService<MainWindow>();
            _window.Activate();
        }
    }
}
