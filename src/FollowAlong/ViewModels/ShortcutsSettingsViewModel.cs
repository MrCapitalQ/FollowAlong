using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MrCapitalQ.FollowAlong.Core.AppData;
using MrCapitalQ.FollowAlong.Core.Keyboard;
using MrCapitalQ.FollowAlong.Messages;
using MrCapitalQ.FollowAlong.Shared;

namespace MrCapitalQ.FollowAlong.ViewModels;

internal partial class ShortcutsSettingsViewModel : ObservableObject
{
    private readonly ISettingsService _settingsService;
    private readonly IMessenger _messenger;

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

    public ShortcutsSettingsViewModel(ISettingsService settingsService, IMessenger messenger)
    {
        _settingsService = settingsService;
        _messenger = messenger;

        _messenger.Register<ShortcutsSettingsViewModel, ShortcutChangedMessage>(this, (r, m) =>
        {
            switch (m.ShortcutKind)
            {
                case AppShortcutKind.StartStop:
                    StartStopShortcut = new(m.ShortcutKeys);
                    break;
                case AppShortcutKind.ToggleTracking:
                    ToggleTrackingShortcut = new(m.ShortcutKeys);
                    break;
                case AppShortcutKind.ZoomIn:
                    ZoomInShortcut = new(m.ShortcutKeys);
                    break;
                case AppShortcutKind.ZoomOut:
                    ZoomOutShortcut = new(m.ShortcutKeys);
                    break;
                case AppShortcutKind.ResetZoom:
                    ResetZoomShortcut = new(m.ShortcutKeys);
                    break;
            }
        });

        StartStopShortcut = new(_settingsService.GetShortcutKeys(AppShortcutKind.StartStop));
        ToggleTrackingShortcut = new(_settingsService.GetShortcutKeys(AppShortcutKind.ToggleTracking));
        ZoomInShortcut = new(_settingsService.GetShortcutKeys(AppShortcutKind.ZoomIn));
        ZoomOutShortcut = new(_settingsService.GetShortcutKeys(AppShortcutKind.ZoomOut));
        ResetZoomShortcut = new(_settingsService.GetShortcutKeys(AppShortcutKind.ResetZoom));
    }

    [RelayCommand]
    private void ChangeStartStopShortcut() => ChangeShortcut(AppShortcutKind.StartStop, StartStopShortcut.VirtualKeys.ToShortcutKeys());

    [RelayCommand]
    private void ChangeToggleTrackingShortcut() => ChangeShortcut(AppShortcutKind.ToggleTracking, ToggleTrackingShortcut.VirtualKeys.ToShortcutKeys());

    [RelayCommand]
    private void ChangeZoomInShortcut() => ChangeShortcut(AppShortcutKind.ZoomIn, ZoomInShortcut.VirtualKeys.ToShortcutKeys());

    [RelayCommand]
    private void ChangeZoomOutShortcut() => ChangeShortcut(AppShortcutKind.ZoomOut, ZoomOutShortcut.VirtualKeys.ToShortcutKeys());

    [RelayCommand]
    private void ChangeResetZoomShortcut() => ChangeShortcut(AppShortcutKind.ResetZoom, ResetZoomShortcut.VirtualKeys.ToShortcutKeys());

    private void ChangeShortcut(AppShortcutKind shortcutKind, ShortcutKeys currentShortcutKeys)
        => _messenger.Send(new ShowChangeShortcutDialogMessage(shortcutKind, currentShortcutKeys));

    partial void OnStartStopShortcutChanged(ShortcutKeysViewModel value)
        => _settingsService.SetShortcutKeys(AppShortcutKind.StartStop, value.VirtualKeys.ToShortcutKeys());

    partial void OnToggleTrackingShortcutChanged(ShortcutKeysViewModel value)
        => _settingsService.SetShortcutKeys(AppShortcutKind.ToggleTracking, value.VirtualKeys.ToShortcutKeys());

    partial void OnZoomInShortcutChanged(ShortcutKeysViewModel value)
        => _settingsService.SetShortcutKeys(AppShortcutKind.ZoomIn, value.VirtualKeys.ToShortcutKeys());

    partial void OnZoomOutShortcutChanged(ShortcutKeysViewModel value)
        => _settingsService.SetShortcutKeys(AppShortcutKind.ZoomOut, value.VirtualKeys.ToShortcutKeys());

    partial void OnResetZoomShortcutChanged(ShortcutKeysViewModel value)
        => _settingsService.SetShortcutKeys(AppShortcutKind.ResetZoom, value.VirtualKeys.ToShortcutKeys());
}
