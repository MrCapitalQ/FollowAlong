using System.Drawing;
using Windows.Graphics;

namespace MrCapitalQ.FollowAlong.Infrastructure.Utils;

public static class RectangleExtensions
{
    public static Rectangle ToRectangle(this RectInt32 rect) => new(rect.X, rect.Y, rect.Width, rect.Height);
    public static SizeInt32 ToSizeInt32(this Rectangle rect) => new(rect.Width, rect.Height);
}
