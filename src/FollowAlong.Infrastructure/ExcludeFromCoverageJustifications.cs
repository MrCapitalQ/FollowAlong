namespace MrCapitalQ.FollowAlong.Infrastructure;

public class ExcludeFromCoverageJustifications
{
    public const string RequiresUIThread = "Requires tests with UI thread support but are currently not working. https://github.com/microsoft/TemplateStudio/issues/4711";
    public const string RequiresPackageContext = "Requires context of a packaged application.";
    public const string NativeCalls = "Uses P/Invoke calls.";
}
