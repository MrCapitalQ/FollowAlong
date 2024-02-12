using Windows.Foundation;
using Windows.Graphics;

namespace MrCapitalQ.FollowAlong.Core.Utils
{
    public static class RectInt32Extensions
    {
        public static Rect ToRect(this RectInt32 rect) => new(rect.X, rect.Y, rect.Width, rect.Height);
    }
}
