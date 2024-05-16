using MrCapitalQ.FollowAlong.Core.HotKeys;
using MrCapitalQ.FollowAlong.Core.Utils;
using Windows.System;

namespace MrCapitalQ.FollowAlong.Core.Tests.HotKeys;

public class HotKeysServiceTests
{
    private readonly IWindowMessageMonitor _windowMessageMonitor;
    private readonly IHotKeysInterops _hotKeysInterops;

    private readonly HotKeysService _hotKeysService;

    public HotKeysServiceTests()
    {
        _windowMessageMonitor = Substitute.For<IWindowMessageMonitor>();
        _hotKeysInterops = Substitute.For<IHotKeysInterops>();

        _hotKeysService = new(_windowMessageMonitor, _hotKeysInterops);
    }

    [Fact]
    public void RegisterHotKeys_Unregistered_RegistersHotKeysAndInitializesWindowMessageMonitor()
    {
        var windowHwnd = new IntPtr(1);
        var modifiers = ModifierKeys.Control | ModifierKeys.Shift | ModifierKeys.Alt;
        _hotKeysInterops.RegisterHotKey(Arg.Any<IntPtr>(), Arg.Any<int>(), Arg.Any<uint>(), Arg.Any<uint>())
            .Returns(true);

        _hotKeysService.RegisterHotKeys(windowHwnd);

        _hotKeysInterops.Received(1).RegisterHotKey(windowHwnd, (int)HotKeyType.StartStop, (uint)modifiers, (uint)VirtualKey.F);
        _hotKeysInterops.Received(1).RegisterHotKey(windowHwnd, (int)HotKeyType.ZoomIn, (uint)modifiers, (uint)AdditionalKeys.Plus);
        _hotKeysInterops.Received(1).RegisterHotKey(windowHwnd, (int)HotKeyType.ZoomOut, (uint)modifiers, (uint)AdditionalKeys.Minus);
        _hotKeysInterops.Received(1).RegisterHotKey(windowHwnd, (int)HotKeyType.ToggleTracking, (uint)modifiers, (uint)VirtualKey.P);
        _windowMessageMonitor.Received(1).Init(windowHwnd);
    }

    [Fact]
    public void RegisterHotKeys_RegistrationFails_RaisesHotKeyRegistrationFailedEvent()
    {
        var expectedFailed = new List<HotKeyType>
        {
            HotKeyType.StartStop,
            HotKeyType.ZoomIn,
            HotKeyType.ZoomOut,
            HotKeyType.ToggleTracking
        };

        _hotKeysService.RegisterHotKeys(new IntPtr(1));

        Assert.Equivalent(expectedFailed, _hotKeysService.HotKeyRegistrationFailures);
    }

    [Fact]
    public void RegisterHotKeys_AlreadyRegistered_ThrowsException()
    {
        var windowHwnd = new IntPtr(1);
        _hotKeysService.RegisterHotKeys(windowHwnd);

        var ex = Assert.Throws<InvalidOperationException>(() => _hotKeysService.RegisterHotKeys(windowHwnd));

        Assert.Equal("This service can only be registered to one window at a time.", ex.Message);
    }

    [Fact]
    public void Unregister_WhenRegistered_UnregistersHotKeysAndResetsWindowMessageMonitor()
    {
        var windowHwnd = new IntPtr(1);
        _hotKeysInterops.RegisterHotKey(Arg.Any<IntPtr>(), Arg.Any<int>(), Arg.Any<uint>(), Arg.Any<uint>())
            .Returns(true);
        _hotKeysService.RegisterHotKeys(windowHwnd);

        _hotKeysService.Unregister();

        _hotKeysInterops.Received(1).UnregisterHotKey(windowHwnd, (int)HotKeyType.StartStop);
        _hotKeysInterops.Received(1).UnregisterHotKey(windowHwnd, (int)HotKeyType.ZoomIn);
        _hotKeysInterops.Received(1).UnregisterHotKey(windowHwnd, (int)HotKeyType.ZoomOut);
        _windowMessageMonitor.Received(1).Reset();
    }

    [Fact]
    public void Unregister_WhenUnregistered_ResetsWindowMessageMonitor()
    {
        _hotKeysService.Unregister();

        _hotKeysInterops.DidNotReceiveWithAnyArgs().UnregisterHotKey(Arg.Any<IntPtr>(), Arg.Any<int>());
        _windowMessageMonitor.Received(1).Reset();
    }

    [Fact]
    public void Dispose_ResetsWindowMessageMonitor()
    {
        _hotKeysService.Dispose();

        _windowMessageMonitor.Received(1).Reset();
    }

    [Fact]
    public void WindowMessageMonitorWindowMessageReceived_DisplayChangedMessageWhenRegistered_RaisesHotKeyInvokedEvent()
    {
        uint hotKeyMessageId = 0x0312;
        var windowHwnd = new IntPtr(1);
        _hotKeysService.RegisterHotKeys(windowHwnd);
        HotKeyType? hotkeyTypeInvoked = null;
        _hotKeysService.HotKeyInvoked += (_, e) => hotkeyTypeInvoked = e.HotKeyType;

        _windowMessageMonitor.WindowMessageReceived += Raise.EventWith(new WindowMessageEventArgs(windowHwnd,
            hotKeyMessageId,
            (int)HotKeyType.ZoomIn,
            0));

        Assert.Equal(HotKeyType.ZoomIn, hotkeyTypeInvoked);
    }

    [Fact]
    public void WindowMessageMonitorWindowMessageReceived_OtherWindowMessagesWhenRegistered_DoesNothing()
    {
        uint messageId = 1;
        var windowHwnd = new IntPtr(1);
        _hotKeysService.RegisterHotKeys(windowHwnd);
        HotKeyType? hotkeyTypeInvoked = null;
        _hotKeysService.HotKeyInvoked += (_, e) => hotkeyTypeInvoked = e.HotKeyType;

        _windowMessageMonitor.WindowMessageReceived += Raise.EventWith(new WindowMessageEventArgs(windowHwnd,
            messageId,
            0,
            0));

        Assert.Null(hotkeyTypeInvoked);
    }

    [Fact]
    public void WindowMessageMonitorWindowMessageReceived_AnyWindowMessageWhenNotRegistered_DoesNothing()
    {
        uint hotKeyMessageId = 0x0312;
        HotKeyType? hotkeyTypeInvoked = null;
        _hotKeysService.HotKeyInvoked += (_, e) => hotkeyTypeInvoked = e.HotKeyType;

        _windowMessageMonitor.WindowMessageReceived += Raise.EventWith(new WindowMessageEventArgs(new IntPtr(1),
            hotKeyMessageId,
            (int)HotKeyType.ZoomIn,
            0));

        Assert.Null(hotkeyTypeInvoked);
    }
}
