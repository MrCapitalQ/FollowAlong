using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace MrCapitalQ.FollowAlong.Infrastructure.HotKeys;

[ExcludeFromCodeCoverage(Justification = "Adapter class for native interops that can't be mocked.")]
public partial class HotKeysInteropsAdapter : IHotKeysInterops
{
    public bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk)
        => HotKeyInterops.RegisterHotKey(hWnd, id, fsModifiers, vk);

    public bool UnregisterHotKey(IntPtr hWnd, int id) => HotKeyInterops.UnregisterHotKey(hWnd, id);

    private partial class HotKeyInterops
    {
        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool UnregisterHotKey(IntPtr hWnd, int id);
    }
}
