using Windows.Foundation;
using Windows.Graphics;

namespace MrCapitalQ.FollowAlong.Core.Utils;

public static class RectExtensions
{
    public static Rect ToRect(this RectInt32 rect) => new(rect.X, rect.Y, rect.Width, rect.Height);
    public static SizeInt32 ToSizeInt32(this Rect rect) => new((int)rect.Width, (int)rect.Height);
}
