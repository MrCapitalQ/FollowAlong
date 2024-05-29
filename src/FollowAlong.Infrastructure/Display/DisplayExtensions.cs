using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System.Diagnostics.CodeAnalysis;

namespace MrCapitalQ.FollowAlong.Infrastructure.Display;

[ExcludeFromCodeCoverage(Justification = "Uses native static APIs that can't be mocked.")]
public static class DisplayExtensions
{
    public static DisplayItem? GetCurrentDisplayItem(this Window window)
    {
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
        var displayArea = DisplayArea.GetFromWindowId(new WindowId((ulong)hwnd), DisplayAreaFallback.Nearest);
        return new DisplayItem(displayArea.IsPrimary,
            displayArea.OuterBounds,
            displayArea.WorkArea,
            displayArea.DisplayId.Value);
    }
}
