using MrCapitalQ.FollowAlong.Infrastructure;
using System.Diagnostics.CodeAnalysis;
using WinUIEx;

namespace MrCapitalQ.FollowAlong;

[ExcludeFromCodeCoverage(Justification = ExcludeFromCoverageJustifications.RequiresUIThread)]
public abstract class WindowBase : WindowEx
{
    public WindowBase() => AppWindow.SetIcon(Icon);

    public virtual string Icon => "Assets/AppIcon.ico";
}