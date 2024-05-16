using CommunityToolkit.Mvvm.Messaging;
using MrCapitalQ.FollowAlong.Core;
using MrCapitalQ.FollowAlong.Core.Utils;
using MrCapitalQ.FollowAlong.Messages;
using System.Diagnostics.CodeAnalysis;
using System.Timers;
using Timer = System.Timers.Timer;

namespace MrCapitalQ.FollowAlong.Services;

[ExcludeFromCodeCoverage(Justification = ExcludeFromCoverageJustifications.RequiresUIThread)]
internal sealed class UpdateSynchronizer : IUpdateSynchronizer, IDisposable
{
    public event EventHandler? UpdateRequested;

    private const int UpdatesPerSecond = 60;
    private readonly Timer _timer;

    public UpdateSynchronizer(IMessenger messenger)
    {
        _timer = new Timer(TimeSpan.FromSeconds(1d / UpdatesPerSecond).TotalMilliseconds);
        _timer.Elapsed += Timer_Elapsed;

        messenger.Register<UpdateSynchronizer, StartCapture>(this, (r, messenger) => r._timer.Start());
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
