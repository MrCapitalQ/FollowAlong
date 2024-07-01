namespace MrCapitalQ.FollowAlong.Core.Capture;

public interface IGraphicsCreator<T>
{
    IGraphics CreateFrom(T image);
}
