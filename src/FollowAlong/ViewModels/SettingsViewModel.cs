using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI.Helpers;
using MrCapitalQ.FollowAlong.Core.Startup;
using MrCapitalQ.FollowAlong.Shared;

namespace MrCapitalQ.FollowAlong.ViewModels;

internal partial class SettingsViewModel : ObservableObject
{
    private readonly IStartupTaskService _startupTaskService;
    private readonly IPackageInfo _packageInfo;

    private bool _isStartupOn;

    [ObservableProperty]
    private bool _isStartupToggleEnabled;

    [ObservableProperty]
    private string _startupSettingsText = string.Empty;

    public SettingsViewModel(IStartupTaskService startupTaskService, IPackageInfo packageInfo)
    {
        _startupTaskService = startupTaskService;
        _packageInfo = packageInfo;

        UpdateStartupState();
        AppDisplayName = packageInfo.DisplayName;
        Version = packageInfo.Version.ToFormattedString(3);
    }

    public bool IsStartupOn
    {
        get => _isStartupOn;
        set => UpdateStartupState(value);
    }

    public string AppDisplayName { get; }
    public string Version { get; }

    public IEnumerable<ExternalLinkViewModel> GeneralLinks =
        [
            new("Project GitHub page", "https://github.com/MrCapitalQ/FollowAlong")
        ];

    public IEnumerable<ExternalLinkViewModel> OpenSourceLibraryLinks =
        [
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
}
