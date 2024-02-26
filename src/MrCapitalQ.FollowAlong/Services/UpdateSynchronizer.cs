using MrCapitalQ.FollowAlong.Core.Tracking;
using System;
using System.Timers;

namespace MrCapitalQ.FollowAlong.Services
{
    internal sealed class UpdateSynchronizer : IUpdateSynchronizer
    {
        public event EventHandler? UpdateRequested;

        private const int UpdatesPerSecond = 60;
        private readonly Timer _timer;

        public UpdateSynchronizer()
        {
            _timer = new Timer(TimeSpan.FromSeconds(1d / UpdatesPerSecond).TotalMilliseconds);
            _timer.Elapsed += Timer_Elapsed;
            _timer.Start();
        }

        private void OnUpdateRequested()
        {
            var raiseEvent = UpdateRequested;
            raiseEvent?.Invoke(this, new());
        }

        private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
            => App.Current.Window?.DispatcherQueue?.TryEnqueue(() => OnUpdateRequested());
    }
}
