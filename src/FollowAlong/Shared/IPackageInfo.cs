using Windows.ApplicationModel;

namespace MrCapitalQ.FollowAlong.Shared;

public interface IPackageInfo
{
    string DisplayName { get; }
    PackageVersion Version { get; }
}
