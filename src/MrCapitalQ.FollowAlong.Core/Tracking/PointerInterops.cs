using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Windows.Foundation;

namespace MrCapitalQ.FollowAlong.Core.Tracking
{
    [ExcludeFromCodeCoverage(Justification = "Native interops.")]
    internal class PointerInterops
    {
        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out POINT lpPoint);

        public static Point? GetCursorPosition()
        {
            if (GetCursorPos(out var lpPoint))
                return lpPoint;

            return null;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int X;
            public int Y;

            public static implicit operator Point(POINT point) => new(point.X, point.Y);
        }
    }
}
