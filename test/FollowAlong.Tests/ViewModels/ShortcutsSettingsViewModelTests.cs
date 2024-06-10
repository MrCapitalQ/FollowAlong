using CommunityToolkit.Mvvm.Messaging;
using MrCapitalQ.FollowAlong.Core.AppData;
using MrCapitalQ.FollowAlong.Core.Keyboard;
using MrCapitalQ.FollowAlong.Messages;
using MrCapitalQ.FollowAlong.Shared;
using MrCapitalQ.FollowAlong.ViewModels;

namespace MrCapitalQ.FollowAlong.Tests.ViewModels;

public class ShortcutsSettingsViewModelTests
{
    private readonly ISettingsService _settingsService;
    private readonly IMessenger _messenger;

    private readonly ShortcutsSettingsViewModel _viewModel;

    private MessageHandler<ShortcutsSettingsViewModel, ShortcutChangedMessage>? _shortcutChangedMessageHandler;

    public ShortcutsSettingsViewModelTests()
    {
        _settingsService = Substitute.For<ISettingsService>();
        _messenger = Substitute.For<IMessenger>();

        _settingsService.GetShortcutKeys(AppShortcutKind.StartStop).Returns(AppShortcutKind.StartStop.GetDefaultShortcutKeys());
        _settingsService.GetShortcutKeys(AppShortcutKind.ZoomIn).Returns(AppShortcutKind.ZoomIn.GetDefaultShortcutKeys());
        _settingsService.GetShortcutKeys(AppShortcutKind.ZoomOut).Returns(AppShortcutKind.ZoomOut.GetDefaultShortcutKeys());
        _settingsService.GetShortcutKeys(AppShortcutKind.ResetZoom).Returns(AppShortcutKind.ResetZoom.GetDefaultShortcutKeys());
        _settingsService.GetShortcutKeys(AppShortcutKind.ToggleTracking).Returns(AppShortcutKind.ToggleTracking.GetDefaultShortcutKeys());

        _messenger.When(x => x.Register(Arg.Any<ShortcutsSettingsViewModel>(), Arg.Any<TestMessengerToken>(), Arg.Any<MessageHandler<ShortcutsSettingsViewModel, ShortcutChangedMessage>>()))
            .Do(x => _shortcutChangedMessageHandler = (MessageHandler<ShortcutsSettingsViewModel, ShortcutChangedMessage>)x[2]);

        _viewModel = new(_settingsService, _messenger);
    }

    [Fact]
    public void Ctor_InitializesFromSettingsService()
    {
        _settingsService.ClearReceivedCalls();

        var viewModel = new ShortcutsSettingsViewModel(_settingsService, _messenger);

        Assert.Equal(AppShortcutKind.StartStop.GetDefaultShortcutKeys(), viewModel.StartStopShortcut.VirtualKeys.ToShortcutKeys());
        Assert.Equal(AppShortcutKind.ZoomIn.GetDefaultShortcutKeys(), viewModel.ZoomInShortcut.VirtualKeys.ToShortcutKeys());
        Assert.Equal(AppShortcutKind.ZoomOut.GetDefaultShortcutKeys(), viewModel.ZoomOutShortcut.VirtualKeys.ToShortcutKeys());
        Assert.Equal(AppShortcutKind.ResetZoom.GetDefaultShortcutKeys(), viewModel.ResetZoomShortcut.VirtualKeys.ToShortcutKeys());
        Assert.Equal(AppShortcutKind.ToggleTracking.GetDefaultShortcutKeys(), viewModel.ToggleTrackingShortcut.VirtualKeys.ToShortcutKeys());
        _settingsService.Received(1).GetShortcutKeys(AppShortcutKind.StartStop);
        _settingsService.Received(1).GetShortcutKeys(AppShortcutKind.ZoomIn);
        _settingsService.Received(1).GetShortcutKeys(AppShortcutKind.ZoomOut);
        _settingsService.Received(1).GetShortcutKeys(AppShortcutKind.ResetZoom);
        _settingsService.Received(1).GetShortcutKeys(AppShortcutKind.ToggleTracking);
    }

    [Fact]
    public void ChangeStartStopShortcutCommand_SendsShowChangeShortcutDialogMessage()
    {
        var message = new ShowChangeShortcutDialogMessage(AppShortcutKind.StartStop, AppShortcutKind.StartStop.GetDefaultShortcutKeys());

        _viewModel.ChangeStartStopShortcutCommand.Execute(null);

        _messenger.Received(1).Send(message, Arg.Any<TestMessengerToken>());
    }

    [Fact]
    public void ChangeZoomInShortcutCommand_SendsShowChangeShortcutDialogMessage()
    {
        var message = new ShowChangeShortcutDialogMessage(AppShortcutKind.ZoomIn, AppShortcutKind.ZoomIn.GetDefaultShortcutKeys());

        _viewModel.ChangeZoomInShortcutCommand.Execute(null);

        _messenger.Received(1).Send(message, Arg.Any<TestMessengerToken>());
    }

    [Fact]
    public void ChangeZoomOutShortcutCommand_SendsShowChangeShortcutDialogMessage()
    {
        var message = new ShowChangeShortcutDialogMessage(AppShortcutKind.ZoomOut, AppShortcutKind.ZoomOut.GetDefaultShortcutKeys());

        _viewModel.ChangeZoomOutShortcutCommand.Execute(null);

        _messenger.Received(1).Send(message, Arg.Any<TestMessengerToken>());
    }

    [Fact]
    public void ChangeResetZoomShortcutCommand_SendsShowChangeShortcutDialogMessage()
    {
        var message = new ShowChangeShortcutDialogMessage(AppShortcutKind.ResetZoom, AppShortcutKind.ResetZoom.GetDefaultShortcutKeys());

        _viewModel.ChangeResetZoomShortcutCommand.Execute(null);

        _messenger.Received(1).Send(message, Arg.Any<TestMessengerToken>());
    }

    [Fact]
    public void ChangeToggleTrackingShortcutCommand_SendsShowChangeShortcutDialogMessage()
    {
        var message = new ShowChangeShortcutDialogMessage(AppShortcutKind.ToggleTracking, AppShortcutKind.ToggleTracking.GetDefaultShortcutKeys());

        _viewModel.ChangeToggleTrackingShortcutCommand.Execute(null);

        _messenger.Received(1).Send(message, Arg.Any<TestMessengerToken>());
    }

    [Fact]
    public void ShortcutChangedMessageReceived_StartStop_UpdatesShortcut()
    {
        var expected = new ShortcutKeys(ModifierKeys.Control, PrimaryShortcutKey.Q);

        Assert.NotNull(_shortcutChangedMessageHandler);
        _shortcutChangedMessageHandler.Invoke(_viewModel, new ShortcutChangedMessage(AppShortcutKind.StartStop, expected));

        Assert.Equal(expected, _viewModel.StartStopShortcut.VirtualKeys.ToShortcutKeys());
    }

    [Fact]
    public void ShortcutChangedMessageReceived_ZoomIn_UpdatesShortcut()
    {
        var expected = new ShortcutKeys(ModifierKeys.Control, PrimaryShortcutKey.Q);

        Assert.NotNull(_shortcutChangedMessageHandler);
        _shortcutChangedMessageHandler.Invoke(_viewModel, new ShortcutChangedMessage(AppShortcutKind.ZoomIn, expected));

        Assert.Equal(expected, _viewModel.ZoomInShortcut.VirtualKeys.ToShortcutKeys());
    }

    [Fact]
    public void ShortcutChangedMessageReceived_ZoomOut_UpdatesShortcut()
    {
        var expected = new ShortcutKeys(ModifierKeys.Control, PrimaryShortcutKey.Q);

        Assert.NotNull(_shortcutChangedMessageHandler);
        _shortcutChangedMessageHandler.Invoke(_viewModel, new ShortcutChangedMessage(AppShortcutKind.ZoomOut, expected));

        Assert.Equal(expected, _viewModel.ZoomOutShortcut.VirtualKeys.ToShortcutKeys());
    }

    [Fact]
    public void ShortcutChangedMessageReceived_ResetZoom_UpdatesShortcut()
    {
        var expected = new ShortcutKeys(ModifierKeys.Control, PrimaryShortcutKey.Q);

        Assert.NotNull(_shortcutChangedMessageHandler);
        _shortcutChangedMessageHandler.Invoke(_viewModel, new ShortcutChangedMessage(AppShortcutKind.ResetZoom, expected));

        Assert.Equal(expected, _viewModel.ResetZoomShortcut.VirtualKeys.ToShortcutKeys());
    }

    [Fact]
    public void ShortcutChangedMessageReceived_ToggleTracking_UpdatesShortcut()
    {
        var expected = new ShortcutKeys(ModifierKeys.Control, PrimaryShortcutKey.Q);

        Assert.NotNull(_shortcutChangedMessageHandler);
        _shortcutChangedMessageHandler.Invoke(_viewModel, new ShortcutChangedMessage(AppShortcutKind.ToggleTracking, expected));

        Assert.Equal(expected, _viewModel.ToggleTrackingShortcut.VirtualKeys.ToShortcutKeys());
    }
}
