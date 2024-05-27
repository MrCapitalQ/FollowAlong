namespace MrCapitalQ.FollowAlong.Infrastructure.HotKeys;

public interface IHotKeysInterops
{
    bool RegisterHotKey(nint hWnd, int id, uint fsModifiers, uint vk);
    bool UnregisterHotKey(nint hWnd, int id);
}
