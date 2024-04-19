using System.Diagnostics.CodeAnalysis;
using WinUIEx;

namespace MrCapitalQ.FollowAlong;

[ExcludeFromCodeCoverage]
public abstract class WindowBase : WindowEx
{
    public WindowBase() => AppWindow.SetIcon(Icon);

    public virtual string Icon => "Assets/AppIcon.ico";
}