using Microsoft.UI.Xaml;
using MrCapitalQ.FollowAlong.Core.Capture;
using MrCapitalQ.FollowAlong.Core.Monitors;
using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace MrCapitalQ.FollowAlong
{
    public sealed partial class MainWindow : Window
    {
        private readonly MonitorService _monitorService;
        private readonly BitmapCaptureService _captureService;

        public MainWindow(MonitorService monitorService, BitmapCaptureService captureService)
        {
            _monitorService = monitorService;
            _captureService = captureService;
            InitializeComponent();
        }

        private void CaptureButton_Click(object sender, RoutedEventArgs e)
        {
            var monitor = _monitorService.GetAll().Where(x => x.IsPrimary).First();
            var captureItem = monitor.CreateCaptureItem();
            _captureService.StartCapture(captureItem, Preview);

#if DEBUG
            ExcludeWindowFromCapture();
#endif
        }

        [DllImport("user32.dll")]
        private static extern uint SetWindowDisplayAffinity(IntPtr hwnd, uint dwAffinity);

        private void ExcludeWindowFromCapture()
        {
            //const uint WDA_NONE = 0x00000000;
            //const uint WDA_MONITOR = 0x00000001;
            const uint WDA_EXCLUDEFROMCAPTURE = 0x00000011;


            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            _ = SetWindowDisplayAffinity(hWnd, WDA_EXCLUDEFROMCAPTURE);
        }
    }
}
