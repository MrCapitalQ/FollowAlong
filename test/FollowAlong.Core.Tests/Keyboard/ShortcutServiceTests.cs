using MrCapitalQ.FollowAlong.Core.AppData;
using MrCapitalQ.FollowAlong.Core.Keyboard;
using MrCapitalQ.FollowAlong.Core.Utils;

namespace MrCapitalQ.FollowAlong.Core.Tests.Keyboard;

public class ShortcutServiceTests
{
    private readonly IWindowMessageMonitor _windowMessageMonitor;
    private readonly IHotKeysInterops _hotKeysInterops;
    private readonly ISettingsService _settingsService;

    private readonly ShortcutService _shortcutService;

    public ShortcutServiceTests()
    {
        _windowMessageMonitor = Substitute.For<IWindowMessageMonitor>();
        _hotKeysInterops = Substitute.For<IHotKeysInterops>();
        _settingsService = Substitute.For<ISettingsService>();

        _settingsService.GetShortcutKeys(AppShortcutKind.StartStop).Returns(AppShortcutKind.StartStop.GetDefaultShortcutKeys());
        _settingsService.GetShortcutKeys(AppShortcutKind.ZoomIn).Returns(AppShortcutKind.ZoomIn.GetDefaultShortcutKeys());
        _settingsService.GetShortcutKeys(AppShortcutKind.ZoomOut).Returns(AppShortcutKind.ZoomOut.GetDefaultShortcutKeys());
        _settingsService.GetShortcutKeys(AppShortcutKind.ResetZoom).Returns(AppShortcutKind.ResetZoom.GetDefaultShortcutKeys());
        _settingsService.GetShortcutKeys(AppShortcutKind.ToggleTracking).Returns(AppShortcutKind.ToggleTracking.GetDefaultShortcutKeys());

        _shortcutService = new(_windowMessageMonitor, _hotKeysInterops, _settingsService);
    }

    [Fact]
    public void Register_Unregistered_RegistersShortcutsAndInitializesWindowMessageMonitor()
    {
        var windowHwnd = new nint(1);
        _hotKeysInterops.RegisterHotKey(Arg.Any<nint>(), Arg.Any<int>(), Arg.Any<uint>(), Arg.Any<uint>())
            .Returns(true);

        _shortcutService.Register(windowHwnd);

        _hotKeysInterops.Received(1).RegisterHotKey(windowHwnd,
            (int)AppShortcutKind.StartStop,
            (uint)AppShortcutKind.StartStop.GetDefaultShortcutKeys().ModifierKeys,
            (uint)AppShortcutKind.StartStop.GetDefaultShortcutKeys().Key);
        _hotKeysInterops.Received(1).RegisterHotKey(windowHwnd,
            (int)AppShortcutKind.ZoomIn,
            (uint)AppShortcutKind.ZoomIn.GetDefaultShortcutKeys().ModifierKeys,
            (uint)AppShortcutKind.ZoomIn.GetDefaultShortcutKeys().Key);
        _hotKeysInterops.Received(1).RegisterHotKey(windowHwnd,
            (int)AppShortcutKind.ZoomOut,
            (uint)AppShortcutKind.ZoomOut.GetDefaultShortcutKeys().ModifierKeys,
            (uint)AppShortcutKind.ZoomOut.GetDefaultShortcutKeys().Key);
        _hotKeysInterops.Received(1).RegisterHotKey(windowHwnd,
            (int)AppShortcutKind.ResetZoom,
            (uint)AppShortcutKind.ResetZoom.GetDefaultShortcutKeys().ModifierKeys,
            (uint)AppShortcutKind.ResetZoom.GetDefaultShortcutKeys().Key);
        _hotKeysInterops.Received(1).RegisterHotKey(windowHwnd,
            (int)AppShortcutKind.ToggleTracking,
            (uint)AppShortcutKind.ToggleTracking.GetDefaultShortcutKeys().ModifierKeys,
            (uint)AppShortcutKind.ToggleTracking.GetDefaultShortcutKeys().Key);
        _windowMessageMonitor.Received(1).Init(windowHwnd);
    }

    [Fact]
    public void Register_RegistrationFails_PopulatesFailedShortcutRegistrations()
    {
        var expectedFailed = new List<AppShortcutKind>
        {
            AppShortcutKind.StartStop,
            AppShortcutKind.ZoomIn,
            AppShortcutKind.ZoomOut,
            AppShortcutKind.ResetZoom,
            AppShortcutKind.ToggleTracking
        };

        _shortcutService.Register(new nint(1));

        Assert.Equivalent(expectedFailed, _shortcutService.ShortcutRegistrationFailures);
    }

    [Fact]
    public void Register_AlreadyRegistered_UnregistersBeforeRegistering()
    {
        var windowHwnd = new nint(1);
        _hotKeysInterops.RegisterHotKey(Arg.Any<nint>(), Arg.Any<int>(), Arg.Any<uint>(), Arg.Any<uint>())
            .Returns(true);
        _shortcutService.Register(windowHwnd);
        _hotKeysInterops.ClearReceivedCalls();
        _windowMessageMonitor.ClearReceivedCalls();

        _shortcutService.Register(windowHwnd);

        _hotKeysInterops.Received(1).UnregisterHotKey(windowHwnd, (int)AppShortcutKind.StartStop);
        _hotKeysInterops.Received(1).UnregisterHotKey(windowHwnd, (int)AppShortcutKind.ZoomIn);
        _hotKeysInterops.Received(1).UnregisterHotKey(windowHwnd, (int)AppShortcutKind.ZoomOut);
        _hotKeysInterops.Received(1).UnregisterHotKey(windowHwnd, (int)AppShortcutKind.ResetZoom);
        _hotKeysInterops.Received(1).UnregisterHotKey(windowHwnd, (int)AppShortcutKind.ToggleTracking);
        _windowMessageMonitor.Received(1).Reset();
        _hotKeysInterops.Received(1).RegisterHotKey(windowHwnd,
            (int)AppShortcutKind.StartStop,
            (uint)AppShortcutKind.StartStop.GetDefaultShortcutKeys().ModifierKeys,
            (uint)AppShortcutKind.StartStop.GetDefaultShortcutKeys().Key);
        _hotKeysInterops.Received(1).RegisterHotKey(windowHwnd,
            (int)AppShortcutKind.ZoomIn,
            (uint)AppShortcutKind.ZoomIn.GetDefaultShortcutKeys().ModifierKeys,
            (uint)AppShortcutKind.ZoomIn.GetDefaultShortcutKeys().Key);
        _hotKeysInterops.Received(1).RegisterHotKey(windowHwnd,
            (int)AppShortcutKind.ZoomOut,
            (uint)AppShortcutKind.ZoomOut.GetDefaultShortcutKeys().ModifierKeys,
            (uint)AppShortcutKind.ZoomOut.GetDefaultShortcutKeys().Key);
        _hotKeysInterops.Received(1).RegisterHotKey(windowHwnd,
            (int)AppShortcutKind.ResetZoom,
            (uint)AppShortcutKind.ResetZoom.GetDefaultShortcutKeys().ModifierKeys,
            (uint)AppShortcutKind.ResetZoom.GetDefaultShortcutKeys().Key);
        _hotKeysInterops.Received(1).RegisterHotKey(windowHwnd,
            (int)AppShortcutKind.ToggleTracking,
            (uint)AppShortcutKind.ToggleTracking.GetDefaultShortcutKeys().ModifierKeys,
            (uint)AppShortcutKind.ToggleTracking.GetDefaultShortcutKeys().Key);
        _windowMessageMonitor.Received(1).Init(windowHwnd);
    }

    [Fact]
    public void Unregister_WhenRegistered_UnregistersHotKeysAndResetsWindowMessageMonitor()
    {
        var windowHwnd = new nint(1);
        _hotKeysInterops.RegisterHotKey(Arg.Any<nint>(), Arg.Any<int>(), Arg.Any<uint>(), Arg.Any<uint>())
            .Returns(true);
        _shortcutService.Register(windowHwnd);
        _hotKeysInterops.ClearReceivedCalls();
        _windowMessageMonitor.ClearReceivedCalls();

        _shortcutService.Unregister();

        _hotKeysInterops.Received(1).UnregisterHotKey(windowHwnd, (int)AppShortcutKind.StartStop);
        _hotKeysInterops.Received(1).UnregisterHotKey(windowHwnd, (int)AppShortcutKind.ZoomIn);
        _hotKeysInterops.Received(1).UnregisterHotKey(windowHwnd, (int)AppShortcutKind.ZoomOut);
        _hotKeysInterops.Received(1).UnregisterHotKey(windowHwnd, (int)AppShortcutKind.ResetZoom);
        _hotKeysInterops.Received(1).UnregisterHotKey(windowHwnd, (int)AppShortcutKind.ToggleTracking);
        _windowMessageMonitor.Received(1).Reset();
    }

    [Fact]
    public void Unregister_WhenUnregistered_ResetsWindowMessageMonitor()
    {
        _shortcutService.Unregister();

        _hotKeysInterops.DidNotReceiveWithAnyArgs().UnregisterHotKey(Arg.Any<nint>(), Arg.Any<int>());
        _windowMessageMonitor.Received(1).Reset();
    }

    [Fact]
    public void Dispose_ResetsWindowMessageMonitor()
    {
        _shortcutService.Dispose();

        _windowMessageMonitor.Received(1).Reset();
    }

    [Fact]
    public void WindowMessageMonitorWindowMessageReceived_DisplayChangedMessageWhenRegistered_RaisesAppShortcutInvokedEvent()
    {
        uint hotKeyMessageId = 0x0312;
        var windowHwnd = new nint(1);
        _shortcutService.Register(windowHwnd);
        AppShortcutKind? shortcutKindInvoked = null;
        _shortcutService.ShortcutInvoked += (_, e) => shortcutKindInvoked = e.ShortcutKind;

        _windowMessageMonitor.WindowMessageReceived += Raise.EventWith(new WindowMessageEventArgs(windowHwnd,
            hotKeyMessageId,
            (int)AppShortcutKind.ZoomIn,
            0));

        Assert.Equal(AppShortcutKind.ZoomIn, shortcutKindInvoked);
    }

    [Fact]
    public void WindowMessageMonitorWindowMessageReceived_OtherWindowMessagesWhenRegistered_DoesNothing()
    {
        uint messageId = 1;
        var windowHwnd = new nint(1);
        _shortcutService.Register(windowHwnd);
        AppShortcutKind? shortcutKindInvoked = null;
        _shortcutService.ShortcutInvoked += (_, e) => shortcutKindInvoked = e.ShortcutKind;

        _windowMessageMonitor.WindowMessageReceived += Raise.EventWith(new WindowMessageEventArgs(windowHwnd,
            messageId,
            0,
            0));

        Assert.Null(shortcutKindInvoked);
    }

    [Fact]
    public void WindowMessageMonitorWindowMessageReceived_AnyWindowMessageWhenNotRegistered_DoesNothing()
    {
        uint hotKeyMessageId = 0x0312;
        AppShortcutKind? shortcutKindInvoked = null;
        _shortcutService.ShortcutInvoked += (_, e) => shortcutKindInvoked = e.ShortcutKind;

        _windowMessageMonitor.WindowMessageReceived += Raise.EventWith(new WindowMessageEventArgs(new nint(1),
            hotKeyMessageId,
            (int)AppShortcutKind.ZoomIn,
            0));

        Assert.Null(shortcutKindInvoked);
    }
}
