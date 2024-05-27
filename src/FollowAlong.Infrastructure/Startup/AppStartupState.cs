namespace MrCapitalQ.FollowAlong.Infrastructure.Startup;

public enum AppStartupState
{
    Disabled,
    DisabledByUser,
    Enabled,
    DisabledByPolicy,
    EnabledByPolicy
}
