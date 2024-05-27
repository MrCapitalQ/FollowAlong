namespace MrCapitalQ.FollowAlong.Infrastructure.Display;

public interface IDisplayService
{
    IEnumerable<DisplayItem> GetAll();
}