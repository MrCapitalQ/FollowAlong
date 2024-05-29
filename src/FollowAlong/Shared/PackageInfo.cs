using MrCapitalQ.FollowAlong.Infrastructure;
using System.Diagnostics.CodeAnalysis;
using Windows.ApplicationModel;

namespace MrCapitalQ.FollowAlong.Shared;

[ExcludeFromCodeCoverage(Justification = ExcludeFromCoverageJustifications.RequiresPackageContext)]
internal class PackageInfo : IPackageInfo
{
    public string DisplayName => Package.Current.GetAppListEntries()[0].DisplayInfo.DisplayName;

    public PackageVersion Version => Package.Current.Id.Version;
}
