using CommunityToolkit.Mvvm.Messaging;
using MrCapitalQ.FollowAlong.Infrastructure.Capture;
using MrCapitalQ.FollowAlong.Infrastructure.Display;
using MrCapitalQ.FollowAlong.Infrastructure.HotKeys;
using MrCapitalQ.FollowAlong.Messages;
using MrCapitalQ.FollowAlong.Pages;
using MrCapitalQ.FollowAlong.ViewModels;
using System.Drawing;

namespace MrCapitalQ.FollowAlong.Tests.ViewModels;

public class MainViewModelTests
{
    private readonly DisplayItem _primaryDisplayItem = new(true, new(10, 10, 10, 10), new(), 1);
    private readonly IDisplayService _displayService;
    private readonly IHotKeysService _hotKeysService;
    private readonly IScreenshotService _screenshotService;
    private readonly IDisplayCaptureItemCreator _displayCaptureItemCreator;
    private readonly IMessenger _messenger;

    private readonly MainViewModel _mainViewModel;

    private MessageHandler<MainViewModel, StopCapture>? _stopCaptureMessageHandler;

    public MainViewModelTests()
    {
        _displayService = Substitute.For<IDisplayService>();
        _hotKeysService = Substitute.For<IHotKeysService>();
        _screenshotService = Substitute.For<IScreenshotService>();
        _displayCaptureItemCreator = Substitute.For<IDisplayCaptureItemCreator>();
        _messenger = Substitute.For<IMessenger>();

        _displayService.GetAll().Returns([_primaryDisplayItem]);
        _messenger.When(x => x.Register(Arg.Any<MainViewModel>(), Arg.Any<TestMessengerToken>(), Arg.Any<MessageHandler<MainViewModel, StopCapture>>()))
            .Do(x => _stopCaptureMessageHandler = (MessageHandler<MainViewModel, StopCapture>)x[2]);

        _mainViewModel = new MainViewModel(_displayService,
            _hotKeysService,
            _screenshotService,
            _displayCaptureItemCreator,
            _messenger);
    }

    [InlineData(HotKeyType.StartStop, "Start and stop keyboard shortcut could not be registered.")]
    [InlineData(HotKeyType.ZoomIn, "Zoom in keyboard shortcut could not be registered.")]
    [InlineData(HotKeyType.ZoomOut, "Zoom out keyboard shortcut could not be registered.")]
    [InlineData(HotKeyType.ResetZoom, "Reset zoom keyboard shortcut could not be registered.")]
    [InlineData(HotKeyType.ToggleTracking, "Pause and resume tracking keyboard shortcut could not be registered.")]
    [InlineData((HotKeyType)1000, "HotKeyType 1000 keyboard shortcut could not be registered.")]
    [Theory]
    public void Ctor_PopulatesFailedHotKeyRegistrations(HotKeyType hotKeyType, string expectedMessage)
    {
        _hotKeysService.HotKeyRegistrationFailures.Returns(new HotKeyType[] { hotKeyType }.AsReadOnly());

        var mainViewModel = new MainViewModel(_displayService,
            _hotKeysService,
            _screenshotService,
            _displayCaptureItemCreator,
            _messenger);

        var alertViewModel = Assert.Single(mainViewModel.Alerts);
        Assert.Equal(expectedMessage, alertViewModel.Message);
    }

    [Fact]
    public void Load_LoadsDisplays()
    {
        _mainViewModel.Load();

        var expectedSelectedDisplay = Assert.Single(_mainViewModel.Displays);
        Assert.Equal(expectedSelectedDisplay, _mainViewModel.SelectedDisplay);
        _screenshotService.Received(1).GetDisplayImageAsync(_primaryDisplayItem, Arg.Any<Size>());
    }

    [Fact]
    public void StartCommand_DisplaySelected_SendsStartCaptureMessage()
    {
        var displayCaptureItem = Substitute.For<IDisplayCaptureItem>();
        _displayCaptureItemCreator.Create(_primaryDisplayItem).Returns(displayCaptureItem);
        _mainViewModel.Load();

        _mainViewModel.StartCommand.Execute(null);

        _messenger.Received(1).Send(Arg.Is<StartCapture>(x => x.CaptureItem == displayCaptureItem), Arg.Any<TestMessengerToken>());
    }

    [Fact]
    public void StartCommand_NoDisplaySelected_DoesNothing()
    {
        _mainViewModel.SelectedDisplay = null;

        _mainViewModel.StartCommand.Execute(null);

        _messenger.DidNotReceive().Send(Arg.Any<StartCapture>(), Arg.Any<TestMessengerToken>());
    }

    [Fact]
    public void SettingsCommand_SendsNavigateMessage()
    {
        var navigateMessage = new EntranceNavigateMessage(typeof(SettingsPage));

        _mainViewModel.SettingsCommand.Execute(null);

        _messenger.Received(1).Send<NavigateMessage, TestMessengerToken>(navigateMessage, Arg.Any<TestMessengerToken>());
    }

    [Fact]
    public void StopMessageReceived_ReloadsDisplaysAndRetainsSelectedDisplay()
    {
        var additionalDisplayItem = new DisplayItem(false, new(10, 10, 10, 10), new(), 2);
        _displayService.GetAll().Returns([additionalDisplayItem, _primaryDisplayItem]);
        _mainViewModel.Load();
        _screenshotService.ClearReceivedCalls();

        Assert.NotNull(_stopCaptureMessageHandler);
        _stopCaptureMessageHandler.Invoke(_mainViewModel, StopCapture.Empty);

        Assert.Equal(2, _mainViewModel.Displays.Count);
        Assert.Equal(_mainViewModel.Displays[1], _mainViewModel.SelectedDisplay);
        _screenshotService.Received(1).GetDisplayImageAsync(additionalDisplayItem, Arg.Any<Size>());
        _screenshotService.Received(1).GetDisplayImageAsync(_primaryDisplayItem, Arg.Any<Size>());
    }
}
