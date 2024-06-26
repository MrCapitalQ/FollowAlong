using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using MrCapitalQ.FollowAlong.Core.Display;
using MrCapitalQ.FollowAlong.Infrastructure.Utils;
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
            displayArea.OuterBounds.ToRectangle(),
            displayArea.WorkArea.ToRectangle(),
            displayArea.DisplayId.Value);
    }
}
