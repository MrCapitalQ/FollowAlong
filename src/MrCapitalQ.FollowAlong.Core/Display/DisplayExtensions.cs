using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Windows.Graphics.Capture;

namespace MrCapitalQ.FollowAlong.Core.Display
{
    [ExcludeFromCodeCoverage]
    public static class DisplayExtensions
    {
        private static readonly Guid s_graphicsCaptureItemGuid = new("79C3F95B-31F7-4EC2-A464-632EF5D30760");

        public static GraphicsCaptureItem CreateCaptureItem(this DisplayItem displayItem)
        {
            var interop = GraphicsCaptureItem.As<IGraphicsCaptureItemInterop>();
            var itemPointer = interop.CreateForMonitor((nint)displayItem.DisplayId, s_graphicsCaptureItemGuid);
            var item = GraphicsCaptureItem.FromAbi(itemPointer);
            Marshal.Release(itemPointer);

            return item;
        }

        public static DisplayItem? GetCurrentDisplayItem(this Window window)
        {
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            var displayArea = DisplayArea.GetFromWindowId(new WindowId((ulong)hwnd), DisplayAreaFallback.Nearest);
            return new DisplayItem(displayArea.IsPrimary,
                displayArea.OuterBounds,
                displayArea.WorkArea,
                displayArea.DisplayId.Value);
        }

        [ComImport]
        [Guid("3628E81B-3CAC-4C60-B7F4-23CE0E0C3356")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [ComVisible(true)]
        private interface IGraphicsCaptureItemInterop
        {
            nint CreateForWindow([In] nint window, [In] ref Guid iid);
            nint CreateForMonitor([In] nint monitor, [In] ref Guid iid);
        }
    }
}
