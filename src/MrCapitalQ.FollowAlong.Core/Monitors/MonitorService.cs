using System.Collections.Generic;

namespace MrCapitalQ.FollowAlong.Core.Monitors
{
    public class MonitorService
    {
        public IEnumerable<MonitorInfo> GetAll() => MonitorInterops.GetMonitors();
    }
}
