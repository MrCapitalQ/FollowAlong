using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MrCapitalQ.FollowAlong.Core.Capture;
using MrCapitalQ.FollowAlong.Core.Display;
using MrCapitalQ.FollowAlong.Core.HotKeys;
using MrCapitalQ.FollowAlong.Messages;
using System.Collections.ObjectModel;

namespace MrCapitalQ.FollowAlong.ViewModels;

internal partial class MainViewModel : ObservableObject
{
    private readonly IDisplayService _displayService;
    private readonly IScreenshotService _screenshotService;
    private readonly IDisplayCaptureItemCreator _displayCaptureItemCreator;
    private readonly IMessenger _messenger;

    [ObservableProperty]
    private ObservableCollection<DisplayViewModel> _displays = [];

    [ObservableProperty]
    private DisplayViewModel? _selectedDisplay;

    [ObservableProperty]
    private ObservableCollection<AlertViewModel> _alerts = [];

    public MainViewModel(IDisplayService displayService,
        IHotKeysService hotKeysService,
        IScreenshotService screenshotService,
        IDisplayCaptureItemCreator displayCaptureItemCreator,
        IMessenger messenger)
    {
        _displayService = displayService;
        _screenshotService = screenshotService;
        _displayCaptureItemCreator = displayCaptureItemCreator;
        _messenger = messenger;
        _messenger.Register<MainViewModel, StopCapture>(this, (r, m) => r.Load());

        foreach (var hotKeyType in hotKeysService.HotKeyRegistrationFailures)
            AddFailedHotKeyRegistration(hotKeyType);
    }

    public void Load()
    {
        var selectedDisplayId = SelectedDisplay?.DisplayItem.DisplayId;

        Displays = new(_displayService.GetAll().Select(x => new DisplayViewModel(x, _screenshotService)));

        if (selectedDisplayId is not null)
            SelectedDisplay = Displays.FirstOrDefault(x => x.DisplayItem.DisplayId == selectedDisplayId);

        SelectedDisplay ??= Displays.FirstOrDefault(x => x.DisplayItem.IsPrimary);
    }

    [RelayCommand]
    private void Start()
    {
        if (SelectedDisplay is null)
            return;

        _messenger.Send(new StartCapture(_displayCaptureItemCreator.Create(SelectedDisplay.DisplayItem)));
    }

    private void AddFailedHotKeyRegistration(HotKeyType hotKeyType)
    {
        var shortcutTypeDisplayName = hotKeyType switch
        {
            HotKeyType.StartStop => "Start and stop",
            HotKeyType.ZoomIn => "Zoom in",
            HotKeyType.ZoomOut => "Zoom out",
            HotKeyType.ResetZoom => "Reset zoom",
            HotKeyType.ToggleTracking => "Pause and resume tracking",
            _ => $"HotKeyType {hotKeyType}"
        };

        Alerts.Add(AlertViewModel.Warning($"{shortcutTypeDisplayName} keyboard shortcut could not be registered."));
    }
}
