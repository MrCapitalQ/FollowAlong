using System.Collections.Generic;

namespace MrCapitalQ.FollowAlong.Core.Display;

public interface IDisplayService
{
    IEnumerable<DisplayItem> GetAll();
}