namespace MrCapitalQ.FollowAlong.Messages;

public class StopCapture
{
    private static readonly StopCapture s_emptyInstance = new();

    private StopCapture() { }

    public static StopCapture Empty => s_emptyInstance;
};
