using CommunityToolkit.Mvvm.Messaging;
using MrCapitalQ.FollowAlong.Core.AppData;
using MrCapitalQ.FollowAlong.Core.Utils;
using MrCapitalQ.FollowAlong.Infrastructure;
using MrCapitalQ.FollowAlong.Messages;
using System.Diagnostics.CodeAnalysis;
using System.Timers;
using Timer = System.Timers.Timer;

namespace MrCapitalQ.FollowAlong.Services;

[ExcludeFromCodeCoverage(Justification = ExcludeFromCoverageJustifications.RequiresUIThread)]
internal sealed class UpdateSynchronizer : IUpdateSynchronizer, IDisposable
{
    private const double MillisecondsInSeconds = 1000;

    public event EventHandler? UpdateRequested;

    private readonly ISettingsService _settingsService;
    private readonly Timer _timer;

    public UpdateSynchronizer(ISettingsService settingsService, IMessenger messenger)
    {
        _settingsService = settingsService;

        _timer = new Timer();
        _timer.Elapsed += Timer_Elapsed;

        messenger.Register<UpdateSynchronizer, StartCapture>(this, (r, messenger) =>
        {
            r._timer.Interval = MillisecondsInSeconds / r._settingsService.GetUpdatesPerSecond();
            r._timer.Start();
        });
        messenger.Register<UpdateSynchronizer, StopCapture>(this, (r, messenger) => r._timer.Stop());
    }

    private void OnUpdateRequested()
    {
        var raiseEvent = UpdateRequested;
        raiseEvent?.Invoke(this, EventArgs.Empty);
    }

    private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        => App.Current.LifetimeWindow?.DispatcherQueue?.TryEnqueue(OnUpdateRequested);

    public void Dispose() => _timer.Stop();
}
