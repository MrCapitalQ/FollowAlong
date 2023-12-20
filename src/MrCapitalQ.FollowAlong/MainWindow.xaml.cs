using Microsoft.UI.Xaml;
using MrCapitalQ.FollowAlong.Core.Capture;
using MrCapitalQ.FollowAlong.Core.Monitors;
using System.Linq;

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
        }

    }
}
