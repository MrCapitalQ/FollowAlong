using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MrCapitalQ.FollowAlong.Core.AppData;
using MrCapitalQ.FollowAlong.Core.Capture;
using MrCapitalQ.FollowAlong.Core.Display;
using MrCapitalQ.FollowAlong.Core.Keyboard;
using MrCapitalQ.FollowAlong.Messages;
using MrCapitalQ.FollowAlong.Pages;
using MrCapitalQ.FollowAlong.Shared;
using System.Collections.ObjectModel;

namespace MrCapitalQ.FollowAlong.ViewModels;

internal partial class MainViewModel : ObservableObject
{
    private readonly IDisplayService _displayService;
    private readonly IScreenshotService _screenshotService;
    private readonly IDisplayCaptureItemCreator _displayCaptureItemCreator;
    private readonly IMessenger _messenger;
    private readonly ISettingsService _settingsService;

    [ObservableProperty]
    private ObservableCollection<DisplayViewModel> _displays = [];

    [ObservableProperty]
    private DisplayViewModel? _selectedDisplay;

    [ObservableProperty]
    private ObservableCollection<AlertViewModel> _alerts = [];

    public MainViewModel(IDisplayService displayService,
        IShortcutService shortcutService,
        IScreenshotService screenshotService,
        IDisplayCaptureItemCreator displayCaptureItemCreator,
        IMessenger messenger,
        ISettingsService settingsService)
    {
        _displayService = displayService;
        _screenshotService = screenshotService;
        _displayCaptureItemCreator = displayCaptureItemCreator;
        _messenger = messenger;
        _settingsService = settingsService;
        _messenger.Register<MainViewModel, StopCapture>(this, (r, m) => r.Load());

        foreach (var shortcutKind in shortcutService.ShortcutRegistrationFailures)
            AddFailedShortcutRegistration(shortcutKind);
    }

    public string StartToolTip => _settingsService.GetShortcutKeys(AppShortcutKind.StartStop).ToDisplayString();

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

    [RelayCommand]
    private void Settings()
    {
        _messenger.Send<NavigateMessage>(new EntranceNavigateMessage(typeof(SettingsPage)));
    }

    private void AddFailedShortcutRegistration(AppShortcutKind shortcutKind)
    {
        var shortcutTypeDisplayName = shortcutKind switch
        {
            AppShortcutKind.StartStop => "Start and stop",
            AppShortcutKind.ZoomIn => "Zoom in",
            AppShortcutKind.ZoomOut => "Zoom out",
            AppShortcutKind.ResetZoom => "Reset zoom",
            AppShortcutKind.ToggleTracking => "Pause and resume tracking",
            _ => $"AppShortcutKind {shortcutKind}"
        };

        Alerts.Add(AlertViewModel.Warning($"{shortcutTypeDisplayName} keyboard shortcut could not be registered."));
    }
}
