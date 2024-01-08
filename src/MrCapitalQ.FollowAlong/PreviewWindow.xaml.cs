using Microsoft.UI.Xaml;
using MrCapitalQ.FollowAlong.Core.Monitors;
using Windows.Graphics;
using WinUIEx;

namespace MrCapitalQ.FollowAlong
{
    public sealed partial class PreviewWindow : Window
    {
        private readonly static SizeInt32 s_windowSize = new(480, 320);

        public PreviewWindow()
        {
            InitializeComponent();

            this.SetIsShownInSwitchers(false);
            this.SetIsResizable(false);
            this.SetIsMinimizable(false);
            this.SetIsMaximizable(false);
            this.SetIsAlwaysOnTop(true);

            ExtendsContentIntoTitleBar = true;
            AppWindow.Resize(s_windowSize);

            var appMonitor = this.GetWindowMonitorSize();
            if (appMonitor is not null)
                AppWindow.Move(new PointInt32((int)appMonitor.ScreenSize.X - s_windowSize.Width, (int)appMonitor.ScreenSize.Y - s_windowSize.Height));
        }
    }
}
