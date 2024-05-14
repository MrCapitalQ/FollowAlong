﻿using CommunityToolkit.Mvvm.Messaging;
using MrCapitalQ.FollowAlong.Core.HotKeys;
using MrCapitalQ.FollowAlong.Messages;
using MrCapitalQ.FollowAlong.ViewModels;

namespace MrCapitalQ.FollowAlong.Tests.ViewModels;

public class PreviewViewModelTests
{
    private readonly IHotKeysService _hotKeysService;
    private readonly IMessenger _messenger;

    private readonly PreviewViewModel _previewViewModel;

    private MessageHandler<PreviewViewModel, StartCapture>? _startCaptureMessageHandler;

    public PreviewViewModelTests()
    {
        _hotKeysService = Substitute.For<IHotKeysService>();
        _messenger = Substitute.For<IMessenger>();

        _messenger.When(x => x.Register(Arg.Any<PreviewViewModel>(), Arg.Any<TestMessengerToken>(), Arg.Any<MessageHandler<PreviewViewModel, StartCapture>>()))
            .Do(x => _startCaptureMessageHandler = (MessageHandler<PreviewViewModel, StartCapture>)x[2]);

        _previewViewModel = new PreviewViewModel(_hotKeysService, _messenger);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 1)]
    [InlineData(3, 3)]
    [InlineData(4, 3)]
    public void SetZoom_ClampsValueAndSendsZoomChangedMessage(double updateValue, double expectedValue)
    {
        _previewViewModel.Zoom = updateValue;

        Assert.Equal(expectedValue, _previewViewModel.Zoom);
        _messenger.Received(1).Send(new ZoomChanged(expectedValue));
    }

    [Fact]
    public void StopCommand_SendsStopsCaptureMessage()
    {
        _previewViewModel.StopCommand.Execute(null);

        _messenger.Received(1).Send(StopCapture.Empty, Arg.Any<TestMessengerToken>());
    }

    [Fact]
    public void StartMessageReceived_BroadcastsCurrentState()
    {
        _messenger.ClearReceivedCalls();

        Assert.NotNull(_startCaptureMessageHandler);
        _startCaptureMessageHandler.Invoke(_previewViewModel, StartCapture.Empty);

        _messenger.Received(1).Send(new ZoomChanged(_previewViewModel.Zoom), Arg.Any<TestMessengerToken>());
        _messenger.Received(1).Send(new TrackingToggled(_previewViewModel.IsTrackingEnabled), Arg.Any<TestMessengerToken>());
    }

    [Fact]
    public void ZoomInCommand_IncreasesZoom()
    {
        _previewViewModel.ZoomInCommand.Execute(null);

        Assert.Equal(2, _previewViewModel.Zoom);
    }

    [Fact]
    public void ZoomOutCommand_DecreasesZoom()
    {
        _previewViewModel.ZoomOutCommand.Execute(null);

        Assert.Equal(1, _previewViewModel.Zoom);
    }

    [InlineData("1", 1)]
    [InlineData(null, 0.5)]
    [InlineData("not parsable", 0.5)]
    [Theory]
    public void UpdateSessionControlOpacityCommand_UpdatesSessionControlsOpacityWithForValidValues(string? parameterValue, double expectedValue)
    {
        _previewViewModel.UpdateSessionControlOpacityCommand.Execute(parameterValue);

        Assert.Equal(expectedValue, _previewViewModel.SessionControlsOpacity);
    }

    [Fact]
    public void HotKeysService_HotKeyInvoked_ZoomIn_IncreasesZoom()
    {
        _hotKeysService.HotKeyInvoked += Raise.EventWith(new HotKeyInvokedEventArgs(HotKeyType.ZoomIn));

        Assert.Equal(2, _previewViewModel.Zoom);
    }

    [Fact]
    public void HotKeysService_HotKeyInvoked_ZoomOut_DecreasesZoom()
    {
        _hotKeysService.HotKeyInvoked += Raise.EventWith(new HotKeyInvokedEventArgs(HotKeyType.ZoomOut));

        Assert.Equal(1, _previewViewModel.Zoom);
    }

    [Fact]
    public void HotKeysService_HotKeyInvoked_ToggleTracking_TogglesIsTrackingEnabledAndSendsTrackingToggledMessage()
    {
        _hotKeysService.HotKeyInvoked += Raise.EventWith(new HotKeyInvokedEventArgs(HotKeyType.ToggleTracking));

        Assert.False(_previewViewModel.IsTrackingEnabled);
        _messenger.Received(1).Send(new TrackingToggled(false), Arg.Any<TestMessengerToken>());
    }
}