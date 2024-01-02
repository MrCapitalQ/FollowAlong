using CommunityToolkit.Mvvm.ComponentModel;
using MrCapitalQ.FollowAlong.Core.HotKeys;
using MrCapitalQ.FollowAlong.Core.Monitors;
using System.Collections.ObjectModel;
using System.Linq;

namespace MrCapitalQ.FollowAlong
{
    internal partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<MonitorInfo> _monitors = new();

        [ObservableProperty]
        private MonitorInfo? _selectedMonitor;

        public MainViewModel(MonitorService monitorService,
            HotKeysService hotKeysService)
        {
            hotKeysService.HotKeyInvoked += HotKeysService_HotKeyInvoked;

            Monitors = new(monitorService.GetAll());
            SelectedMonitor = Monitors.FirstOrDefault(x => x.IsPrimary);
        }

        private void HotKeysService_HotKeyInvoked(object? sender, HotKeyInvokedEventArgs e)
        {
            if (e.HotKeyType == HotKeyType.StartStop)
            { }
            else if (e.HotKeyType == HotKeyType.ZoomIn)
            { }
            else if (e.HotKeyType == HotKeyType.ZoomOut)
            { }
        }
    }
}
