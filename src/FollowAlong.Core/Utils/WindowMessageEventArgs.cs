namespace MrCapitalQ.FollowAlong.Core.Utils;

public class WindowMessageEventArgs(nint hwnd, uint messageId, nuint wParam, nint lParam) : EventArgs
{
    public nint Hwnd { get; } = hwnd;
    public uint MessageId { get; } = messageId;
    public nuint WParam { get; } = wParam;
    public nint LParam { get; } = lParam;
}
