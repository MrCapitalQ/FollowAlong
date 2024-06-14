using System.Drawing;
using Windows.Graphics;

namespace MrCapitalQ.FollowAlong.Infrastructure.Utils;

public static class RectangleExtensions
{
    public static Rectangle ToRectangle(this RectInt32 rect) => new(rect.X, rect.Y, rect.Width, rect.Height);
    public static SizeInt32 ToSizeInt32(this Rectangle rect) => new(rect.Width, rect.Height);

    public static bool Contains(this RectInt32 rect, Point point)
    {
        if (point.X >= rect.X && point.X - rect.Width <= rect.X && point.Y >= rect.Y)
            return point.Y - rect.Height <= rect.Y;

        return false;
    }
}
