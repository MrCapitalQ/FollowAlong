using MrCapitalQ.FollowAlong.Core.Keyboard;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace MrCapitalQ.FollowAlong.Infrastructure.Keyboard;

[ExcludeFromCodeCoverage(Justification = "Adapter class for native interops that can't be mocked.")]
public partial class HotKeysInteropsAdapter : IHotKeysInterops
{
    public bool RegisterHotKey(nint hWnd, int id, uint fsModifiers, uint vk)
        => HotKeyInterops.RegisterHotKey(hWnd, id, fsModifiers, vk);

    public bool UnregisterHotKey(nint hWnd, int id) => HotKeyInterops.UnregisterHotKey(hWnd, id);

    private partial class HotKeyInterops
    {
        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool RegisterHotKey(nint hWnd, int id, uint fsModifiers, uint vk);

        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool UnregisterHotKey(nint hWnd, int id);
    }
}
