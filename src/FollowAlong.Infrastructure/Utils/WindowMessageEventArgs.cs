namespace MrCapitalQ.FollowAlong.Infrastructure.Utils;

public class WindowMessageEventArgs : EventArgs
{
    public WindowMessageEventArgs(IntPtr hwnd, uint messageId, nuint wParam, nint lParam)
    {
        Hwnd = hwnd;
        MessageId = messageId;
        WParam = wParam;
        LParam = lParam;
    }

    public IntPtr Hwnd { get; }
    public uint MessageId { get; }
    public nuint WParam { get; }
    public nint LParam { get; }
}
