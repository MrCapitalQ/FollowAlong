using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using Windows.Foundation;

namespace MrCapitalQ.FollowAlong.Core.Monitors
{
    internal static class MonitorInterops
    {
        private const int CCHDEVICENAME = 32;
        private delegate bool EnumMonitorsDelegate(nint hMonitor, nint hdcMonitor, ref RECT lprcMonitor, nint dwData);

        [DllImport("user32.dll")]
        private static extern bool EnumDisplayMonitors(nint hdc, nint lprcClip, EnumMonitorsDelegate lpfnEnum, nint dwData);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool GetMonitorInfo(nint hMonitor, ref MonitorInfoEx lpmi);

        public static IEnumerable<MonitorInfo> GetMonitors()
        {
            var result = new List<MonitorInfo>();

            EnumDisplayMonitors(IntPtr.Zero,
                IntPtr.Zero,
                delegate (nint hMonitor, nint hdcMonitor, ref RECT lprcMonitor, nint dwData)
                {
                    var mi = new MonitorInfoEx();
                    mi.Size = Marshal.SizeOf(mi);
                    if (GetMonitorInfo(hMonitor, ref mi))
                    {
                        var info = new MonitorInfo(mi.Flags > 0,
                            new Vector2(mi.Monitor.right - mi.Monitor.left, mi.Monitor.bottom - mi.Monitor.top),
                            new Rect(mi.Monitor.left,
                                mi.Monitor.top,
                                mi.Monitor.right - mi.Monitor.left,
                                mi.Monitor.bottom - mi.Monitor.top),
                            new Rect(mi.WorkArea.left,
                                mi.WorkArea.top,
                                mi.WorkArea.right - mi.WorkArea.left,
                                mi.WorkArea.bottom - mi.WorkArea.top),
                            mi.DeviceName,
                            hMonitor);
                        result.Add(info);
                    }
                    return true;
                },
                IntPtr.Zero);
            return result;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct MonitorInfoEx
        {
            public int Size;
            public RECT Monitor;
            public RECT WorkArea;
            public uint Flags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHDEVICENAME)]
            public string DeviceName;
        }
    }
}
