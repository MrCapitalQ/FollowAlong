using System;

namespace MrCapitalQ.FollowAlong.Core.Tracking
{
    public interface IUpdateSynchronizer
    {
        event EventHandler? UpdateRequested;
    }
}
