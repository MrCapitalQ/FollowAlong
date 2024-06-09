using MrCapitalQ.FollowAlong.Core.Tracking;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Runtime.InteropServices;

namespace MrCapitalQ.FollowAlong.Infrastructure.Tracking;

[ExcludeFromCodeCoverage(Justification = ExcludeFromCoverageJustifications.NativeCalls)]
public partial class PointerService : IPointerService
{
    public Point? GetCurrentPosition() => GetCursorPosition();

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool GetCursorPos(out POINT lpPoint);

    private static Point? GetCursorPosition() => GetCursorPos(out var lpPoint) ? lpPoint : null;

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT
    {
        public int X;
        public int Y;

        public static implicit operator Point(POINT point) => new(point.X, point.Y);
    }
}
