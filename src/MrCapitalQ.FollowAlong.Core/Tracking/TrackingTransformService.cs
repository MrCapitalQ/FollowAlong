using Microsoft.UI.Xaml;
using System;
using System.Numerics;
using System.Timers;
using Windows.Foundation;

namespace MrCapitalQ.FollowAlong.Core.Tracking
{
    public class TrackingTransformService
    {
        private const int UpdatesPerSecond = 60;
        private readonly PointerService _pointerService;
        private readonly Timer _timer;
        private double _horizontalBoundsPercentage = 0.5;
        private double _verticalBoundsPercentage = 0.5;
        private ITrackingTransformTarget? _target;
        private double _currentScale = 1;
        private Point _currentTranslate;

        public TrackingTransformService(PointerService pointerService)
        {
            _pointerService = pointerService;
            _timer = new Timer(TimeSpan.FromSeconds(1.0 / UpdatesPerSecond).TotalMilliseconds);
            _timer.Elapsed += Timer_Elapsed;
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

        public void StartTrackingTransforms(ITrackingTransformTarget target)
        {
            _target = target;
            _target.SizeChanged += Target_SizeChanged;

            UpdateCenterPoint();
            Scale(target.ViewportSize.Height / target.ContentSize.Height);

            _timer.Start();
        }

        public void UpdateTransforms()
        {
            if (_target?.Brush is null)
                return;

            if (_pointerService.GetCurrentPosition() is not Point point)
                return;

            var viewportBounds = GetViewportBounds();

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

            Translate(translateX, translateY);
        }

        public void UpdateCenterPoint()
        {
            if (_target?.Brush is null)
                return;

            _target.Brush.CenterPoint = new Vector2(_target.ViewportSize._width / 2, _target.ViewportSize._height / 2);
        }

        private void Scale(double scale)
        {
            _currentScale = scale;

            if (_target?.Brush is not null)
                _target.Brush.Scale = new Vector2((float)_currentScale);
        }

        private void Translate(double x, double y)
        {
            if (_target?.Brush is null)
                return;

            var translateLimitX = Math.Max((_currentScale * _target.ContentSize.Width) - _target.ViewportSize.Width, 0) / 2 / _currentScale;
            var translateLimitY = Math.Max((_currentScale * _target.ContentSize.Height) - _target.ViewportSize.Height, 0) / 2 / _currentScale;

            _currentTranslate.X = Math.Clamp(x, -translateLimitX, translateLimitX);
            _currentTranslate.Y = Math.Clamp(y, -translateLimitY, translateLimitY);
            _target.Brush.Offset = new Vector2((float)(-_currentTranslate.X * _currentScale),
                (float)(-_currentTranslate.Y * _currentScale));
        }

        private Rect GetViewportBounds()
        {
            if (_target is null)
                return Rect.Empty;

            var boundsAreaWidth = (_target.ViewportSize.Width / _currentScale * (1 - _horizontalBoundsPercentage));
            var boundsAreaHeight = (_target.ViewportSize.Height / _currentScale * (1 - _horizontalBoundsPercentage));

            var viewportBounds = new Rect(((_target.ContentSize.Width - boundsAreaWidth) / 2) + _currentTranslate.X,
                ((_target.ContentSize.Height - boundsAreaHeight) / 2) + _currentTranslate.Y,
                boundsAreaWidth,
                boundsAreaHeight);
            return viewportBounds;
        }

        private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
            => _target?.DispatcherQueue?.TryEnqueue(() => UpdateTransforms());

        private void Target_SizeChanged(object sender, SizeChangedEventArgs e) => UpdateCenterPoint();
    }
}
