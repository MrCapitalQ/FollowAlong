using Microsoft.UI.Xaml;
using MrCapitalQ.FollowAlong.Core;
using MrCapitalQ.FollowAlong.Core.Utils;
using System.Diagnostics.CodeAnalysis;

namespace MrCapitalQ.FollowAlong.Services;

[ExcludeFromCodeCoverage(Justification = JustificationConstants.UIThreadTestExclusionJustification)]
internal sealed class UpdateSynchronizer : IUpdateSynchronizer, IDisposable
{
    public event EventHandler? UpdateRequested;

    private const int UpdatesPerSecond = 60;

    private readonly DispatcherTimer _timer;

    public UpdateSynchronizer()
    {
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1d / UpdatesPerSecond)
        };
        _timer.Tick += Timer_Tick;
        _timer.Start();
    }

    private void OnUpdateRequested()
    {
        var raiseEvent = UpdateRequested;
        raiseEvent?.Invoke(this, EventArgs.Empty);
    }

    private void Timer_Tick(object? sender, object e) => OnUpdateRequested();

    public void Dispose()
    {
        try
        {
            _timer.Stop();
        }
        catch { }
    }
}
