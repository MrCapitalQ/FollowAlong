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

        public static GraphicsCaptureItem CreateCaptureItem(this DisplayArea displayArea)
        {
            var interop = GraphicsCaptureItem.As<IGraphicsCaptureItemInterop>();
            var itemPointer = interop.CreateForMonitor((nint)displayArea.DisplayId.Value, s_graphicsCaptureItemGuid);
            var item = GraphicsCaptureItem.FromAbi(itemPointer);
            Marshal.Release(itemPointer);

            return item;
        }

        public static DisplayArea? GetCurrentDisplayArea(this Window window)
        {
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            return DisplayArea.GetFromWindowId(new WindowId((ulong)hwnd), DisplayAreaFallback.Nearest);
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
