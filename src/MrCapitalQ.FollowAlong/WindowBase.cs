using WinUIEx;

namespace MrCapitalQ.FollowAlong
{
    public abstract class WindowBase : WindowEx
    {
        public WindowBase()
        {
            AppWindow.SetIcon(Icon);
        }

        public string Icon => "Assets/AppIcon.ico";
    }
}