namespace MrCapitalQ.FollowAlong.Messages;

public class UnregisterShortcutsMessage
{
    private static readonly UnregisterShortcutsMessage s_instance = new();

    private UnregisterShortcutsMessage() { }

    public static UnregisterShortcutsMessage Instance => s_instance;
}
