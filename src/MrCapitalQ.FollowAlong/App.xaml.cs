﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using System;

namespace MrCapitalQ.FollowAlong
{
    public partial class App : Application
    {
        public App(IServiceProvider services)
        {
            InitializeComponent();
            Services = services;
        }

        public static new App Current => (App)Application.Current;
        public IServiceProvider Services { get; }
        public Window? Window { get; protected set; }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            Window = Services.GetRequiredService<MainWindow>();
            Window.Activate();
        }
    }
}
