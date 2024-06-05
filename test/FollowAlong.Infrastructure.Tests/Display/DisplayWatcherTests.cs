using MrCapitalQ.FollowAlong.Core.Utils;
using MrCapitalQ.FollowAlong.Infrastructure.Display;

namespace MrCapitalQ.FollowAlong.Infrastructure.Tests.Display;

public class DisplayWatcherTests
{
    private readonly IWindowMessageMonitor _windowMessageMonitor;

    private readonly DisplayWatcher _displayWatcher;

    public DisplayWatcherTests()
    {
        _windowMessageMonitor = Substitute.For<IWindowMessageMonitor>();

        _displayWatcher = new(_windowMessageMonitor);
    }

    [Fact]
    public void Register_Unregistered_InitializesWindowMessageMonitor()
    {
        var windowHwnd = new IntPtr(1);

        _displayWatcher.Register(windowHwnd);

        _windowMessageMonitor.Received(1).Init(windowHwnd);
    }

    [Fact]
    public void Register_AlreadyRegistered_ThrowsException()
    {
        var windowHwnd = new IntPtr(1);
        _displayWatcher.Register(windowHwnd);

        var ex = Assert.Throws<InvalidOperationException>(() => _displayWatcher.Register(windowHwnd));

        Assert.Equal("This service can only be registered to one window at a time.", ex.Message);
    }

    [Fact]
    public void Unregister_ResetsWindowMessageMonitor()
    {
        _displayWatcher.Unregister();

        _windowMessageMonitor.Received(1).Reset();
    }

    [Fact]
    public void Dispose_ResetsWindowMessageMonitor()
    {
        _displayWatcher.Dispose();

        _windowMessageMonitor.Received(1).Reset();
    }

    [Fact]
    public void MonitorWindowMessageReceived_DisplayChangedMessageWhenRegistered_RaisesEvent()
    {
        uint displayChangedMessageId = 0x07E;
        var windowHwnd = new IntPtr(1);
        _displayWatcher.Register(windowHwnd);
        var eventRaised = false;
        _displayWatcher.DisplayChanged += (_, _) => eventRaised = true;

        _windowMessageMonitor.WindowMessageReceived += Raise.EventWith(new WindowMessageEventArgs(windowHwnd,
            displayChangedMessageId,
            0,
            0));

        Assert.True(eventRaised);
    }

    [Fact]
    public void MonitorWindowMessageReceived_OtherWindowMessagesWhenRegistered_DoesNothing()
    {
        uint messageId = 1;
        var windowHwnd = new IntPtr(1);
        _displayWatcher.Register(windowHwnd);
        var eventRaised = false;
        _displayWatcher.DisplayChanged += (_, _) => eventRaised = true;

        _windowMessageMonitor.WindowMessageReceived += Raise.EventWith(new WindowMessageEventArgs(windowHwnd,
            messageId,
            0,
            0));

        Assert.False(eventRaised);
    }

    [Fact]
    public void MonitorWindowMessageReceived_AnyWindowMessageWhenNotRegistered_DoesNothing()
    {
        uint displayChangedMessageId = 0x07E;
        var eventRaised = false;
        _displayWatcher.DisplayChanged += (_, _) => eventRaised = true;

        _windowMessageMonitor.WindowMessageReceived += Raise.EventWith(new WindowMessageEventArgs(new IntPtr(1),
            displayChangedMessageId,
            0,
            0));

        Assert.False(eventRaised);
    }
}
