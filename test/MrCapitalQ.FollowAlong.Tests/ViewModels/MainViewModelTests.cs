using CommunityToolkit.Mvvm.Messaging;
using MrCapitalQ.FollowAlong.Core.Capture;
using MrCapitalQ.FollowAlong.Core.Display;
using MrCapitalQ.FollowAlong.Core.HotKeys;
using MrCapitalQ.FollowAlong.Messages;
using MrCapitalQ.FollowAlong.ViewModels;
using NSubstitute;

namespace MrCapitalQ.FollowAlong.Tests.ViewModels;

public class MainViewModelTests
{
    private readonly DisplayItem _primaryDisplayItem = new(true, new(10, 10, 10, 10), new(), 1);
    private readonly IDisplayService _displayService;
    private readonly IHotKeysService _hotKeysService;
    private readonly IBitmapCaptureService _bitmapCaptureService;
    private readonly IScreenshotService _screenshotService;
    private readonly IDisplayCaptureItemCreator _displayCaptureItemCreator;
    private readonly IMessenger _messenger;

    private readonly MainViewModel _mainViewModel;

    private MessageHandler<MainViewModel, StopCapture>? _stopCaptureMessageHandler;

    public MainViewModelTests()
    {
        _displayService = Substitute.For<IDisplayService>();
        _hotKeysService = Substitute.For<IHotKeysService>();
        _bitmapCaptureService = Substitute.For<IBitmapCaptureService>();
        _screenshotService = Substitute.For<IScreenshotService>();
        _displayCaptureItemCreator = Substitute.For<IDisplayCaptureItemCreator>();
        _messenger = Substitute.For<IMessenger>();

        _displayService.GetAll().Returns([_primaryDisplayItem]);
        _messenger.When(x => x.Register(Arg.Any<MainViewModel>(), Arg.Any<TestToken>(), Arg.Any<MessageHandler<MainViewModel, StopCapture>>()))
            .Do(x => _stopCaptureMessageHandler = (MessageHandler<MainViewModel, StopCapture>)x[2]);

        _mainViewModel = new MainViewModel(_displayService,
            _hotKeysService,
            _bitmapCaptureService,
            _screenshotService,
            _displayCaptureItemCreator,
            _messenger);
    }

    [Fact]
    public void Ctor_InitializesDisplays()
    {
        var expectedSelectedDisplay = Assert.Single(_mainViewModel.Displays);
        Assert.Equal(expectedSelectedDisplay, _mainViewModel.SelectedDisplay);
        Assert.NotNull(_stopCaptureMessageHandler);
        _messenger.Received(1).Register(_mainViewModel, Arg.Any<TestToken>(), Arg.Any<MessageHandler<MainViewModel, StopCapture>>());
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 1)]
    [InlineData(3, 3)]
    [InlineData(4, 3)]
    public void SetZoom_ClampsValueAndSendsZoomChangedMessage(double updateValue, double expectedValue)
    {
        _mainViewModel.Zoom = updateValue;

        Assert.Equal(expectedValue, _mainViewModel.Zoom);
        _messenger.Received(1).Send(new ZoomChanged(expectedValue));
    }

    [Fact]
    public void StartCommand_WhileStopped_StartsCapture()
    {
        var displayCaptureItem = Substitute.For<IDisplayCaptureItem>();
        _displayCaptureItemCreator.Create(_primaryDisplayItem).Returns(displayCaptureItem);

        _mainViewModel.StartCommand.Execute(null);

        Assert.True(_mainViewModel.ShowSessionControls);
        _bitmapCaptureService.Received(1).StartCapture(displayCaptureItem);
        _messenger.Received(1).Send(new ZoomChanged(_mainViewModel.Zoom), Arg.Any<TestToken>());
    }

    [Fact]
    public void StartCommand_WhileStarted_DoesNothing()
    {
        _bitmapCaptureService.IsStarted.Returns(true);

        _mainViewModel.StartCommand.Execute(null);

        _bitmapCaptureService.DidNotReceive().StartCapture(Arg.Any<IDisplayCaptureItem>());
        _messenger.DidNotReceive().Send(Arg.Any<ZoomChanged>(), Arg.Any<TestToken>());
    }

    [Fact]
    public void StartCommand_NoDisplaySelected_DoesNothing()
    {
        _mainViewModel.SelectedDisplay = null;

        _mainViewModel.StartCommand.Execute(null);

        _bitmapCaptureService.DidNotReceive().StartCapture(Arg.Any<IDisplayCaptureItem>());
        _messenger.DidNotReceive().Send(Arg.Any<ZoomChanged>(), Arg.Any<TestToken>());
    }

    [Fact]
    public void StopCommand_WhileStarted_StopsCapture()
    {
        _bitmapCaptureService.IsStarted.Returns(true);
        _screenshotService.ClearReceivedCalls();

        _mainViewModel.StopCommand.Execute(null);

        Assert.False(_mainViewModel.ShowSessionControls);
        _bitmapCaptureService.Received(1).StopCapture();
        _screenshotService.Received(1).GetDisplayImageAsync(_primaryDisplayItem);
    }

    [Fact]
    public void StopCommand_WhileStopped_DoesNothing()
    {
        _screenshotService.ClearReceivedCalls();

        _mainViewModel.StopCommand.Execute(null);

        _bitmapCaptureService.DidNotReceive().StopCapture();
        _screenshotService.DidNotReceive().GetDisplayImageAsync(_primaryDisplayItem);
    }


    [Fact]
    public void StopMessageReceived_WhileStarted_StopsCapture()
    {
        _bitmapCaptureService.IsStarted.Returns(true);
        _screenshotService.ClearReceivedCalls();

        Assert.NotNull(_stopCaptureMessageHandler);
        _stopCaptureMessageHandler.Invoke(_mainViewModel, new());

        Assert.False(_mainViewModel.ShowSessionControls);
        _bitmapCaptureService.Received(1).StopCapture();
        _screenshotService.Received(1).GetDisplayImageAsync(_primaryDisplayItem);
    }

    [Fact]
    public void StopMessageReceived_WhileStopped_DoesNothing()
    {
        _screenshotService.ClearReceivedCalls();

        Assert.NotNull(_stopCaptureMessageHandler);
        _stopCaptureMessageHandler.Invoke(_mainViewModel, new());

        _bitmapCaptureService.DidNotReceive().StopCapture();
        _screenshotService.DidNotReceive().GetDisplayImageAsync(_primaryDisplayItem);
    }

    [Fact]
    public void ZoomInCommand_IncreasesZoom()
    {
        _mainViewModel.ZoomInCommand.Execute(null);

        Assert.Equal(2, _mainViewModel.Zoom);
    }

    [Fact]
    public void ZoomOutCommand_DecreasesZoom()
    {
        _mainViewModel.ZoomOutCommand.Execute(null);

        Assert.Equal(1, _mainViewModel.Zoom);
    }

    [InlineData("1", 1)]
    [InlineData(null, 0.5)]
    [InlineData("not parsable", 0.5)]
    [Theory]
    public void UpdateSessionControlOpacityCommand_UpdatesSessionControlsOpacityWithForValidValues(string? parameterValue, double expectedValue)
    {
        _mainViewModel.UpdateSessionControlOpacityCommand.Execute(parameterValue);

        Assert.Equal(expectedValue, _mainViewModel.SessionControlsOpacity);
    }

    [Fact]
    public void HotKeysService_HotKeyInvoked_StartStopWhileStopped_StartsCapture()
    {
        var displayCaptureItem = Substitute.For<IDisplayCaptureItem>();
        _displayCaptureItemCreator.Create(_primaryDisplayItem).Returns(displayCaptureItem);

        _hotKeysService.HotKeyInvoked += Raise.EventWith(new HotKeyInvokedEventArgs(HotKeyType.StartStop));

        Assert.True(_mainViewModel.ShowSessionControls);
        _bitmapCaptureService.Received(1).StartCapture(displayCaptureItem);
        _messenger.Received(1).Send(new ZoomChanged(_mainViewModel.Zoom), Arg.Any<TestToken>());
    }

    [Fact]
    public void HotKeysService_HotKeyInvoked_StartStopWhileStarted_StopsCapture()
    {
        _bitmapCaptureService.IsStarted.Returns(true);
        _screenshotService.ClearReceivedCalls();

        _hotKeysService.HotKeyInvoked += Raise.EventWith(new HotKeyInvokedEventArgs(HotKeyType.StartStop));

        Assert.False(_mainViewModel.ShowSessionControls);
        _bitmapCaptureService.Received(1).StopCapture();
        _screenshotService.Received(1).GetDisplayImageAsync(_primaryDisplayItem);
    }

    [Fact]
    public void HotKeysService_HotKeyInvoked_ZoomIn_IncreasesZoom()
    {
        _hotKeysService.HotKeyInvoked += Raise.EventWith(new HotKeyInvokedEventArgs(HotKeyType.ZoomIn));

        Assert.Equal(2, _mainViewModel.Zoom);
    }

    [Fact]
    public void HotKeysService_HotKeyInvoked_ZoomOut_DecreasesZoom()
    {
        _hotKeysService.HotKeyInvoked += Raise.EventWith(new HotKeyInvokedEventArgs(HotKeyType.ZoomOut));

        Assert.Equal(1, _mainViewModel.Zoom);
    }

    [Fact]
    public void HotKeysService_HotKeyInvoked_ToggleTracking_TogglesIsTrackingEnabledAndSendsTrackingToggledMessage()
    {
        _hotKeysService.HotKeyInvoked += Raise.EventWith(new HotKeyInvokedEventArgs(HotKeyType.ToggleTracking));

        Assert.False(_mainViewModel.IsTrackingEnabled);
        _messenger.Received(1).Send(new TrackingToggled(false), Arg.Any<TestToken>());
    }

    [InlineData(HotKeyType.StartStop, "Start and stop keyboard shortcut could not be registered.")]
    [InlineData(HotKeyType.ZoomIn, "Zoom in keyboard shortcut could not be registered.")]
    [InlineData(HotKeyType.ZoomOut, "Zoom out keyboard shortcut could not be registered.")]
    [InlineData(HotKeyType.ResetZoom, "Reset zoom keyboard shortcut could not be registered.")]
    [InlineData(HotKeyType.ToggleTracking, "Pause and resume tracking keyboard shortcut could not be registered.")]
    [InlineData((HotKeyType)1000, "HotKeyType 1000 keyboard shortcut could not be registered.")]
    [Theory]
    public void HotKeysService_HotKeyRegistrationFailed_AddsAlertViewModel(HotKeyType hotKeyType, string expectedMessage)
    {
        _hotKeysService.HotKeyRegistrationFailed += Raise.EventWith(new HotKeyRegistrationFailedEventArgs(hotKeyType));

        var alertViewModel = Assert.Single(_mainViewModel.Alerts);
        Assert.Equal(expectedMessage, alertViewModel.Message);
    }

    private class TestToken : Arg.AnyType, IEquatable<TestToken>
    {
        public bool Equals(TestToken? other) => throw new NotImplementedException();
    }
}
