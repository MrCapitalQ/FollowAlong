namespace MrCapitalQ.FollowAlong.Messages;

public class RegisterShortcutsMessage
{
    private static readonly RegisterShortcutsMessage s_instance = new();

    private RegisterShortcutsMessage() { }

    public static RegisterShortcutsMessage Instance => s_instance;
}
