using MrCapitalQ.FollowAlong.Core;
using MrCapitalQ.FollowAlong.Core.Utils;
using System.Diagnostics.CodeAnalysis;
using Windows.ApplicationModel;

namespace MrCapitalQ.FollowAlong;

[ExcludeFromCodeCoverage(Justification = ExcludeFromCoverageJustifications.RequiresUIThread)]
public sealed partial class MainWindow : WindowBase
{
    public MainWindow()
    {
        InitializeComponent();

        Title = Package.Current.GetAppListEntries()[0].DisplayInfo.DisplayName;
        ExtendsContentIntoTitleBar = true;
        PersistenceId = nameof(MainWindow);
        this.SetIsExcludedFromCapture(false);
    }
}
