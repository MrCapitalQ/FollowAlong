using System.Numerics;
using Windows.Foundation;

namespace MrCapitalQ.FollowAlong.Core.Tracking;

public interface ITrackingTransformTarget
{
    event EventHandler ViewportSizeChanged;

    Rect ContentArea { get; }
    Size ViewportSize { get; }

    void SetCenterPoint(Vector2 centerPoint);
    void SetScale(float scale);
    void SetOffset(Vector2 offset);
}
