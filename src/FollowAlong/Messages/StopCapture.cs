namespace MrCapitalQ.FollowAlong.Messages;

public class StopCapture
{
    private static readonly StopCapture s_instance = new();

    private StopCapture() { }

    public static StopCapture Instance => s_instance;
};
