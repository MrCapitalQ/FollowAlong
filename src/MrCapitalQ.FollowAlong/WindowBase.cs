using MrCapitalQ.FollowAlong.Core;
using System.Diagnostics.CodeAnalysis;
using WinUIEx;

namespace MrCapitalQ.FollowAlong;

[ExcludeFromCodeCoverage(Justification = JustificationConstants.UIThreadTestExclusionJustification)]
public abstract class WindowBase : WindowEx
{
    public WindowBase() => AppWindow.SetIcon(Icon);

    public virtual string Icon => "Assets/AppIcon.ico";
}