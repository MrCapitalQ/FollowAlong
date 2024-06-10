using MrCapitalQ.FollowAlong.Core.AppData;
using MrCapitalQ.FollowAlong.Core.Tracking;
using MrCapitalQ.FollowAlong.Core.Utils;
using System.Drawing;
using System.Numerics;

namespace MrCapitalQ.FollowAlong.Core.Tests.Tracking;

public class TrackingTransformServiceTests
{
    private readonly IPointerService _pointerService;
    private readonly ISettingsService _settingsService;
    private readonly IUpdateSynchronizer _synchronizer;
    private readonly ITrackingTransformTarget _target;

    private readonly TrackingTransformService _trackingTransformService;

    public TrackingTransformServiceTests()
    {
        _pointerService = Substitute.For<IPointerService>();
        _settingsService = Substitute.For<ISettingsService>();
        _synchronizer = Substitute.For<IUpdateSynchronizer>();
        _target = Substitute.For<ITrackingTransformTarget>();

        _trackingTransformService = new TrackingTransformService(_pointerService, _settingsService, _synchronizer);
    }

    [Fact]
    public void Zoom_SetValue_ValueUpdated()
    {
        var expected = 2;

        _trackingTransformService.Zoom = expected;

        Assert.Equal(expected, _trackingTransformService.Zoom);
    }

    [Fact]
    public void Zoom_SetValueLessThanOne_ThrowsException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _trackingTransformService.Zoom = 0);
    }

    [Fact]
    public void UpdateLayout_UpdatesCenterPointBasedOnViewportSize()
    {
        _target.ViewportSize.Returns(new Size(100, 100));

        _trackingTransformService.StartTrackingTransforms(_target);

        _target.Received(1).SetCenterPoint(new(50, 50));
    }

    [Theory]
    [MemberData(nameof(GetUpdateRequested_ScalesToViewportParameters))]
    public void UpdateRequested_ScalesToViewport(int viewportWidth, int viewportHeight, float scale)
    {
        _pointerService.GetCurrentPosition().Returns(new Point(400, 300));
        _target.ViewportSize.Returns(new Size(viewportWidth, viewportHeight));
        _target.ContentArea.Returns(new Rectangle(0, 0, 800, 600));
        _trackingTransformService.StartTrackingTransforms(_target);

        _synchronizer.UpdateRequested += Raise.Event();

        _target.Received(1).SetScale(scale);
        _target.Received(1).SetOffset(new Vector2(0));
    }

    public static TheoryData<int, int, float> GetUpdateRequested_ScalesToViewportParameters()
    {
        var data = new TheoryData<int, int, float>
        {
            // If viewport is half size of content in both dimensions,
            // then default scale should be half.
            { 400, 300, 0.5f },

            // If viewport is half height of content and a quarter width of content,
            // then default scale should be half.
            { 200, 300, 0.5f },

            // If viewport is a quarter height of content and half width of content,
            // then default scale should be a quarter.
            { 400, 150, 0.5f }
        };
        return data;
    }


    [Theory]
    [MemberData(nameof(GetUpdateRequested_OffsetsContentForPointerPositionParameters))]
    public void UpdateRequested_OffsetsContentForPointerPosition(int pointerX,
        int pointerY,
        int viewportWidth,
        int viewportHeight,
        float offsetX,
        float offsetY)
    {
        _pointerService.GetCurrentPosition().Returns(new Point(pointerX, pointerY));
        _target.ViewportSize.Returns(new Size(viewportWidth, viewportHeight));
        _target.ContentArea.Returns(new Rectangle(0, 0, 800, 600));
        _trackingTransformService.Zoom = 2;
        _trackingTransformService.StartTrackingTransforms(_target);

        _synchronizer.UpdateRequested += Raise.Event();

        _target.Received(1).SetScale(1);
        _target.Received(1).SetOffset(new Vector2(offsetX, offsetY));
    }

    public static TheoryData<int, int, int, int, float, float> GetUpdateRequested_OffsetsContentForPointerPositionParameters()
    {
        var data = new TheoryData<int, int, int, int, float, float>
        {
            // If pointer is in top left corner and viewport is half size of content with 2x zoom,
            // then scale is cancels out to 1x with offset to bottom right.
            { 0, 0, 400, 300, 200, 150 },

            // If pointer is in bottom right corner and viewport is half size of content with 2x zoom,
            // then scale is cancels out to 1x with offset to top left.
            { 800, 600, 400, 300, -200, -150 }
        };
        return data;
    }

    [Fact]
    public void UpdateRequested_TargetNotSet_DoesNothing()
    {
        _target.ViewportSize.Returns(new Size(400, 300));
        _target.ContentArea.Returns(new Rectangle(0, 0, 800, 600));

        _synchronizer.UpdateRequested += Raise.Event();

        _target.DidNotReceiveWithAnyArgs().SetScale(default);
        _target.DidNotReceiveWithAnyArgs().SetOffset(default);
    }

    [Fact]
    public void UpdateRequested_TrackingIsNotEnabled_TranslateUsingPausedPointerPosition()
    {
        // When tracking enabled, move pointer to top left and verify offset is updated accordingly.
        _target.ViewportSize.Returns(new Size(400, 300));
        _target.ContentArea.Returns(new Rectangle(0, 0, 800, 600));
        _pointerService.GetCurrentPosition().Returns(new Point(0, 0));
        _trackingTransformService.Zoom = 2;
        _trackingTransformService.StartTrackingTransforms(_target);

        _synchronizer.UpdateRequested += Raise.Event();

        _target.Received(1).SetScale(1);
        _target.Received(1).SetOffset(new Vector2(200, 150));

        _target.ClearReceivedCalls();

        // With tracking disabled, move pointer to bottom right and verify offset is updated based on previous
        // pointer position.
        _pointerService.GetCurrentPosition().Returns(new Point(800, 600));
        _trackingTransformService.IsTrackingEnabled = false;

        _synchronizer.UpdateRequested += Raise.Event();

        _target.Received(1).SetScale(1);
        _target.Received(1).SetOffset(new Vector2(200, 150));
    }

    [Fact]
    public void UpdateRequested_TrackingStopped_DoesNothing()
    {
        _pointerService.GetCurrentPosition().Returns(new Point(400, 300));
        _target.ViewportSize.Returns(new Size(400, 300));
        _target.ContentArea.Returns(new Rectangle(0, 0, 800, 600));
        _trackingTransformService.StartTrackingTransforms(_target);
        _trackingTransformService.StopTrackingTransforms();

        _synchronizer.UpdateRequested += Raise.Event();

        _target.DidNotReceiveWithAnyArgs().SetScale(default);
        _target.DidNotReceiveWithAnyArgs().SetOffset(default);
    }

    [Fact]
    public void UpdateLayout_TargetNotSet_DoesNothing()
    {
        _trackingTransformService.UpdateLayout();

        _target.DidNotReceiveWithAnyArgs().SetCenterPoint(default);
    }

    [Fact]
    public void TargetSizeChanged_UpdatesCenterPoint()
    {
        _target.ViewportSize.Returns(new Size(400, 300));
        _trackingTransformService.StartTrackingTransforms(_target);
        _target.ViewportSize.Returns(new Size(200, 200));

        _target.ViewportSizeChanged += Raise.Event();

        _target.Received(1).SetCenterPoint(new(200, 150));
        _target.Received(1).SetCenterPoint(new(100, 100));
    }
}
