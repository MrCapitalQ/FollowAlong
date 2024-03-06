using MrCapitalQ.FollowAlong.Core.Tracking;
using MrCapitalQ.FollowAlong.Core.Utils;
using NSubstitute;
using System.Numerics;
using Windows.Foundation;

namespace MrCapitalQ.FollowAlong.Core.Tests.Tracking
{
    public class TrackingTransformServiceTests
    {
        private readonly IPointerService _pointerService;
        private readonly IUpdateSynchronizer _synchronizer;
        private readonly ITrackingTransformTarget _target;

        private readonly TrackingTransformService _trackingTransformService;

        public TrackingTransformServiceTests()
        {
            _pointerService = Substitute.For<IPointerService>();
            _synchronizer = Substitute.For<IUpdateSynchronizer>();
            _target = Substitute.For<ITrackingTransformTarget>();

            _trackingTransformService = new TrackingTransformService(_pointerService, _synchronizer);
        }

        [Fact]
        public void HorizontalBoundsPercentage_SetValueInBounds_ValueUpdated()
        {
            var expected = 1;

            _trackingTransformService.HorizontalBoundsPercentage = expected;

            Assert.Equal(expected, _trackingTransformService.HorizontalBoundsPercentage);
        }

        [Fact]
        public void HorizontalBoundsPercentage_SetNegativeValue_ThrowsException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _trackingTransformService.HorizontalBoundsPercentage = -1);
        }

        [Fact]
        public void HorizontalBoundsPercentage_SetValueOverOne_ThrowsException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _trackingTransformService.HorizontalBoundsPercentage = 2);
        }

        [Fact]
        public void VerticalBoundsPercentage_SetValueInBounds_ValueUpdated()
        {
            var expected = 1;

            _trackingTransformService.VerticalBoundsPercentage = expected;

            Assert.Equal(expected, _trackingTransformService.VerticalBoundsPercentage);
        }

        [Fact]
        public void VerticalBoundsPercentage_SetNegativeValue_ThrowsException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _trackingTransformService.VerticalBoundsPercentage = -1);
        }

        [Fact]
        public void VerticalBoundsPercentage_SetValueOverOne_ThrowsException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _trackingTransformService.VerticalBoundsPercentage = 2);
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
        public void UpdateRequested_ScalesToViewport(Size viewportSize, float scale)
        {
            _pointerService.GetCurrentPosition().Returns(new Point(400, 300));
            _target.ViewportSize.Returns(viewportSize);
            _target.ContentArea.Returns(new Rect(0, 0, 800, 600));
            _trackingTransformService.StartTrackingTransforms(_target);

            _synchronizer.UpdateRequested += Raise.Event();

            _target.Received(1).SetScale(scale);
            _target.Received(1).SetOffset(new Vector2(0));
        }

        public static TheoryData<Size, float> GetUpdateRequested_ScalesToViewportParameters()
        {
            var data = new TheoryData<Size, float>
            {
                // If viewport is half size of content in both dimensions,
                // then default scale should be half.
                { new Size(400, 300), 0.5f },

                // If viewport is half height of content and a quarter width of content,
                // then default scale should be half.
                { new Size(200, 300), 0.5f },

                // If viewport is a quarter height of content and half width of content,
                // then default scale should be a quarter.
                { new Size(400, 150), 0.5f }
            };
            return data;
        }


        [Theory]
        [MemberData(nameof(GetUpdateRequested_OffsetsContentForPointerPositionParameters))]
        public void UpdateRequested_OffsetsContentForPointerPosition(Point pointerPosition,
            Size viewportSize,
            Vector2 offset)
        {
            _pointerService.GetCurrentPosition().Returns(pointerPosition);
            _target.ViewportSize.Returns(viewportSize);
            _target.ContentArea.Returns(new Rect(0, 0, 800, 600));
            _trackingTransformService.Zoom = 2;
            _trackingTransformService.StartTrackingTransforms(_target);

            _synchronizer.UpdateRequested += Raise.Event();

            _target.Received(1).SetScale(1);
            _target.Received(1).SetOffset(offset);
        }

        public static TheoryData<Point, Size, Vector2> GetUpdateRequested_OffsetsContentForPointerPositionParameters()
        {
            var data = new TheoryData<Point, Size, Vector2>
            {
                // If pointer is in top left corner and viewport is half size of content with 2x zoom,
                // then scale is cancels out to 1x with offset to bottom right.
                { new Point(0, 0), new Size(400, 300), new Vector2(200, 150) },

                // If pointer is in bottom right corner and viewport is half size of content with 2x zoom,
                // then scale is cancels out to 1x with offset to top left.
                { new Point(800, 600), new Size(400, 300), new Vector2(-200, -150) }
            };
            return data;
        }

        [Fact]
        public void UpdateRequested_TargetNotSet_DoesNothing()
        {
            _target.ViewportSize.Returns(new Size(400, 300));
            _target.ContentArea.Returns(new Rect(0, 0, 800, 600));

            _synchronizer.UpdateRequested += Raise.Event();

            _target.DidNotReceiveWithAnyArgs().SetScale(default);
            _target.DidNotReceiveWithAnyArgs().SetOffset(default);
        }

        [Fact]
        public void UpdateRequested_TrackingIsNotEnabled_TranslateUsingPausedPointerPosition()
        {
            // When tracking enabled, move pointer to top left and verify offset is updated accordingly.
            _target.ViewportSize.Returns(new Size(400, 300));
            _target.ContentArea.Returns(new Rect(0, 0, 800, 600));
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
            _target.ContentArea.Returns(new Rect(0, 0, 800, 600));
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
}
