using MrCapitalQ.FollowAlong.Core.Utils;
using System;
using System.Numerics;
using Windows.Foundation;

namespace MrCapitalQ.FollowAlong.Core.Tracking
{
    public class TrackingTransformService
    {
        private readonly IPointerService _pointerService;
        private readonly IUpdateSynchronizer _synchronizer;
        private double _zoom = 1;
        private double _horizontalBoundsPercentage = 0.5;
        private double _verticalBoundsPercentage = 0.5;
        private ITrackingTransformTarget? _target;
        private double _currentScale = 1;
        private Point _currentTranslate;
        private Point _latestPointerPosition;

        public TrackingTransformService(IPointerService pointerService, IUpdateSynchronizer synchronizer)
        {
            _pointerService = pointerService;
            _synchronizer = synchronizer;
            _synchronizer.UpdateRequested += Synchronizer_UpdateRequested;
        }

        public double HorizontalBoundsPercentage
        {
            get => _horizontalBoundsPercentage;
            set
            {
                if (value is < 0 or > 1)
                    throw new ArgumentOutOfRangeException(nameof(value), $"Value must be between 0 and 1 inclusively.");
                _horizontalBoundsPercentage = value;
            }
        }

        public double VerticalBoundsPercentage
        {
            get => _verticalBoundsPercentage;
            set
            {
                if (value is < 0 or > 1)
                    throw new ArgumentOutOfRangeException(nameof(value), $"Value must be between 0 and 1 inclusively.");
                _verticalBoundsPercentage = value;
            }
        }

        public double Zoom
        {
            get => _zoom;
            set
            {
                if (value is < 1)
                    throw new ArgumentOutOfRangeException(nameof(value), $"Value must be greater than or equal to 1.");
                _zoom = value;
            }
        }

        public bool IsTrackingEnabled { get; set; } = true;

        public void StartTrackingTransforms(ITrackingTransformTarget target)
        {
            StopTrackingTransforms();

            _target = target;
            _target.ViewportSizeChanged += Target_SizeChanged;

            UpdateLayout();
        }

        public void StopTrackingTransforms()
        {
            if (_target is null)
                return;

            _target.ViewportSizeChanged -= Target_SizeChanged;
            _target = null;
        }

        public void UpdateLayout()
        {
            if (_target is null)
                return;

            _target.SetCenterPoint(new(_target.ViewportSize._width / 2, _target.ViewportSize._height / 2));
        }

        private void Scale()
        {
            if (_target is null)
                return;

            var contentAspectRatio = _target.ContentArea.Width / _target.ContentArea.Height;
            var viewportAspectRatio = _target.ViewportSize.Width / _target.ViewportSize.Height;
            var baseScale = contentAspectRatio > viewportAspectRatio
                ? _target.ViewportSize.Height / _target.ContentArea.Height
                : _target.ViewportSize.Width / _target.ContentArea.Width;

            _currentScale = _zoom * baseScale;

            _target.SetScale((float)_currentScale);
        }

        private void Translate()
        {
            if (_pointerService.GetCurrentPosition() is Point currentPoint && IsTrackingEnabled)
                _latestPointerPosition = currentPoint;

            if (_target is null)
                return;

            var point = new Point(_latestPointerPosition.X - _target.ContentArea.Left, _latestPointerPosition.Y - _target.ContentArea.Top);

            var viewportBounds = GetViewportBounds(_target);

            var translateX = _currentTranslate.X;
            if (point.X < viewportBounds.Left)
                translateX = _currentTranslate.X + point.X - viewportBounds.Left;
            else if (point.X > viewportBounds.Right)
                translateX = _currentTranslate.X + point.X - viewportBounds.Right;

            var translateY = _currentTranslate.Y;
            if (point.Y < viewportBounds.Top)
                translateY = _currentTranslate.Y + point.Y - viewportBounds.Top;
            else if (point.Y > viewportBounds.Bottom)
                translateY = _currentTranslate.Y + point.Y - viewportBounds.Bottom;

            var translateLimitX = Math.Max((_currentScale * _target.ContentArea.Width) - _target.ViewportSize.Width, 0) / 2 / _currentScale;
            var translateLimitY = Math.Max((_currentScale * _target.ContentArea.Height) - _target.ViewportSize.Height, 0) / 2 / _currentScale;

            _currentTranslate.X = Math.Clamp(translateX, -translateLimitX, translateLimitX);
            _currentTranslate.Y = Math.Clamp(translateY, -translateLimitY, translateLimitY);
            _target.SetOffset(new Vector2((float)(-_currentTranslate.X * _currentScale),
                (float)(-_currentTranslate.Y * _currentScale)));
        }

        private Rect GetViewportBounds(ITrackingTransformTarget target)
        {
            var boundsAreaWidth = (target.ViewportSize.Width / _currentScale * (1 - _horizontalBoundsPercentage));
            var boundsAreaHeight = (target.ViewportSize.Height / _currentScale * (1 - _horizontalBoundsPercentage));

            var viewportBounds = new Rect(((target.ContentArea.Width - boundsAreaWidth) / 2) + _currentTranslate.X,
                ((target.ContentArea.Height - boundsAreaHeight) / 2) + _currentTranslate.Y,
                boundsAreaWidth,
                boundsAreaHeight);
            return viewportBounds;
        }

        private void Synchronizer_UpdateRequested(object? sender, EventArgs e)
        {
            Scale();
            Translate();
        }

        private void Target_SizeChanged(object? sender, EventArgs e) => UpdateLayout();
    }
}
