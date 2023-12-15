using System;
using System.Runtime.InteropServices;

namespace MrCapitalQ.FollowAlong.Core.HotKeys
{
    internal class HotKeyInterops
    {
        [DllImport("user32.dll")]
        internal static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        internal static extern bool UnregisterHotKey(IntPtr hWnd, int id);
    }
}
