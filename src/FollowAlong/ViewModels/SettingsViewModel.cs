using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.WinUI.Helpers;
using Microsoft.UI.Xaml.Media.Animation;
using MrCapitalQ.FollowAlong.Core.AppData;
using MrCapitalQ.FollowAlong.Core.Startup;
using MrCapitalQ.FollowAlong.Core.Tracking;
using MrCapitalQ.FollowAlong.Messages;
using MrCapitalQ.FollowAlong.Pages;
using MrCapitalQ.FollowAlong.Shared;
using System.Globalization;

namespace MrCapitalQ.FollowAlong.ViewModels;

internal partial class SettingsViewModel : ObservableObject
{
    private readonly IStartupTaskService _startupTaskService;
    private readonly ISettingsService _settingsService;
    private readonly IMessenger _messenger;

    private bool _isStartupOn;

    [ObservableProperty]
    private bool _isStartupToggleEnabled;

    [ObservableProperty]
    private string _startupSettingsText = string.Empty;

    private double _zoomDefaultLevel;

    [ObservableProperty]
    private ComboBoxOption<double> _selectedZoomStepSize;

    public SettingsViewModel(IStartupTaskService startupTaskService,
        ISettingsService settingsService,
        IPackageInfo packageInfo,
        IMessenger messenger)
    {
        _startupTaskService = startupTaskService;
        _settingsService = settingsService;
        _messenger = messenger;

        UpdateStartupState();

        ZoomDefaultLevel = _settingsService.GetZoomDefaultLevel();
        SelectedZoomStepSize = ZoomStepSizeOptions.FirstOrDefault(x => x.Value == _settingsService.GetZoomStepSize())
            ?? ZoomStepSizeOptions.First(x => x.Value == 0.5);

        AppDisplayName = packageInfo.DisplayName;
        Version = packageInfo.Version.ToFormattedString(3);
    }

    public bool IsStartupOn
    {
        get => _isStartupOn;
        set => UpdateStartupState(value);
    }

    public double ZoomDefaultLevel
    {
        get => _zoomDefaultLevel;
        set
        {
            _zoomDefaultLevel = Math.Clamp(value, TrackingConstants.MinZoom, TrackingConstants.MaxZoom);
            OnPropertyChanged();
            _settingsService.SetZoomDefaultLevel(_zoomDefaultLevel);
        }
    }

    public List<ComboBoxOption<double>> ZoomStepSizeOptions { get; } = new List<double> { 0.05, 0.1, 0.25, 0.5, 1d }
        .Select(x => new ComboBoxOption<double>(x, string.Format(CultureInfo.CurrentUICulture, "{0:P0}", x)))
        .ToList();

    public string AppDisplayName { get; }
    public string Version { get; }

    public IEnumerable<ExternalLinkViewModel> GeneralLinks =
        [
            new("Project GitHub page", "https://github.com/MrCapitalQ/FollowAlong")
        ];

    public IEnumerable<ExternalLinkViewModel> OpenSourceLibraryLinks =
        [
            new(".NET Community Toolkit", "https://github.com/CommunityToolkit/dotnet"),
            new("H.NotifyIcon", "https://github.com/HavenDV/H.NotifyIcon"),
            new("Win2D", "https://github.com/microsoft/Win2D"),
            new("Windows App SDK", "https://github.com/microsoft/WindowsAppSDK"),
            new("Windows Community Toolkit", "https://github.com/CommunityToolkit/Windows"),
            new("WinUI", "https://github.com/microsoft/microsoft-ui-xaml"),
            new("WinUIEx", "https://github.com/dotMorten/WinUIEx")
        ];

    private async void UpdateStartupState(bool? isEnabled = null)
    {
        IsStartupToggleEnabled = false;

        if (isEnabled is not null)
            await _startupTaskService.SetStartupStateAsync(isEnabled.Value);

        var state = await _startupTaskService.GetStartupStateAsync();

        StartupSettingsText = "Start automatically in the background when you sign in";
        switch (state)
        {
            case AppStartupState.DisabledByUser:
                StartupSettingsText = "Startup is disabled at the system level and must be enabled using the Startup tab in Task Manager";
                _isStartupOn = false;
                IsStartupToggleEnabled = false;
                break;
            case AppStartupState.DisabledByPolicy:
                StartupSettingsText = "Startup is disabled by group policy or not supported on this device";
                _isStartupOn = false;
                IsStartupToggleEnabled = false;
                break;
            case AppStartupState.Disabled:
                _isStartupOn = false;
                IsStartupToggleEnabled = true;
                break;
            case AppStartupState.EnabledByPolicy:
                StartupSettingsText = "Startup is enabled by group policy";
                _isStartupOn = true;
                IsStartupToggleEnabled = false;
                break;
            case AppStartupState.Enabled:
                _isStartupOn = true;
                IsStartupToggleEnabled = true;
                break;
        }
        OnPropertyChanged(nameof(IsStartupOn));
    }

    [RelayCommand]
    private void TrackingSettings() => _messenger.Send<NavigateMessage>(new SlideNavigateMessage(typeof(TrackingSettingsPage),
        SlideNavigationTransitionEffect.FromRight));

    [RelayCommand]
    private void ShortcutsSettings() => _messenger.Send<NavigateMessage>(new SlideNavigateMessage(typeof(ShortcutsSettingsPage),
        SlideNavigationTransitionEffect.FromRight));

    [RelayCommand]
    private void IncreaseZoomDefaultLevel() => ZoomDefaultLevel += SelectedZoomStepSize.Value;

    [RelayCommand]
    private void DecreaseZoomDefaultLevel() => ZoomDefaultLevel -= SelectedZoomStepSize.Value;

    partial void OnSelectedZoomStepSizeChanged(ComboBoxOption<double> value) => _settingsService.SetZoomStepSize(value.Value);
}
