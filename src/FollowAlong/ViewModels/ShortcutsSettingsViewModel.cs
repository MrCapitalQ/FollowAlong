using CommunityToolkit.Mvvm.ComponentModel;
using MrCapitalQ.FollowAlong.Core.AppData;
using MrCapitalQ.FollowAlong.Core.Keyboard;

namespace MrCapitalQ.FollowAlong.ViewModels;

internal partial class ShortcutsSettingsViewModel : ObservableObject
{
    private readonly ISettingsService _settingsService;

    [ObservableProperty]
    private ShortcutKeysViewModel _startStopShortcut;

    [ObservableProperty]
    private ShortcutKeysViewModel _toggleTrackingShortcut;

    [ObservableProperty]
    private ShortcutKeysViewModel _zoomInShortcut;

    [ObservableProperty]
    private ShortcutKeysViewModel _zoomOutShortcut;

    [ObservableProperty]
    private ShortcutKeysViewModel _resetZoomShortcut;

    public ShortcutsSettingsViewModel(ISettingsService settingsService)
    {
        _settingsService = settingsService;

        StartStopShortcut = new(_settingsService.GetShortcutKeys(AppShortcutKind.StartStop));
        ToggleTrackingShortcut = new(_settingsService.GetShortcutKeys(AppShortcutKind.ToggleTracking));
        ZoomInShortcut = new(_settingsService.GetShortcutKeys(AppShortcutKind.ZoomIn));
        ZoomOutShortcut = new(_settingsService.GetShortcutKeys(AppShortcutKind.ZoomOut));
        ResetZoomShortcut = new(_settingsService.GetShortcutKeys(AppShortcutKind.ResetZoom));
    }

    partial void OnStartStopShortcutChanged(ShortcutKeysViewModel value)
        => _settingsService.SetShortcutKeys(AppShortcutKind.StartStop, new(value.ModifierKeys, value.Key));

    partial void OnToggleTrackingShortcutChanged(ShortcutKeysViewModel value)
        => _settingsService.SetShortcutKeys(AppShortcutKind.ToggleTracking, new(value.ModifierKeys, value.Key));

    partial void OnZoomInShortcutChanged(ShortcutKeysViewModel value)
        => _settingsService.SetShortcutKeys(AppShortcutKind.ZoomIn, new(value.ModifierKeys, value.Key));

    partial void OnZoomOutShortcutChanged(ShortcutKeysViewModel value)
        => _settingsService.SetShortcutKeys(AppShortcutKind.ZoomOut, new(value.ModifierKeys, value.Key));

    partial void OnResetZoomShortcutChanged(ShortcutKeysViewModel value)
        => _settingsService.SetShortcutKeys(AppShortcutKind.ResetZoom, new(value.ModifierKeys, value.Key));
}
