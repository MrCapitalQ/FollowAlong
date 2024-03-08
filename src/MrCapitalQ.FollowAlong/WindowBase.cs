using WinUIEx;

namespace MrCapitalQ.FollowAlong
{
    public abstract class WindowBase : WindowEx
    {
        public WindowBase()
        {
            AppWindow.SetIcon(Icon);
        }

        public virtual string Icon => "Assets/AppIcon.ico";
    }
}