using MrCapitalQ.FollowAlong.Core.AppData;
using MrCapitalQ.FollowAlong.Core.Keyboard;
using MrCapitalQ.FollowAlong.ViewModels;

namespace MrCapitalQ.FollowAlong.Tests.ViewModels;

public class ShortcutsSettingsViewModelTests
{
    private readonly ISettingsService _settingsService;

    public ShortcutsSettingsViewModelTests()
    {
        _settingsService = Substitute.For<ISettingsService>();
        _settingsService.GetShortcutKeys(AppShortcutKind.StartStop).Returns(AppShortcutKind.StartStop.GetDefaultShortcutKeys());
        _settingsService.GetShortcutKeys(AppShortcutKind.ZoomIn).Returns(AppShortcutKind.ZoomIn.GetDefaultShortcutKeys());
        _settingsService.GetShortcutKeys(AppShortcutKind.ZoomOut).Returns(AppShortcutKind.ZoomOut.GetDefaultShortcutKeys());
        _settingsService.GetShortcutKeys(AppShortcutKind.ResetZoom).Returns(AppShortcutKind.ResetZoom.GetDefaultShortcutKeys());
        _settingsService.GetShortcutKeys(AppShortcutKind.ToggleTracking).Returns(AppShortcutKind.ToggleTracking.GetDefaultShortcutKeys());
    }

    [Fact]
    public void Ctor_InitializesFromSettingsService()
    {
        var viewModel = new ShortcutsSettingsViewModel(_settingsService);

        Assert.Equal(AppShortcutKind.StartStop.GetDefaultShortcutKeys(), new(viewModel.StartStopShortcut.ModifierKeys, viewModel.StartStopShortcut.Key));
        Assert.Equal(AppShortcutKind.ZoomIn.GetDefaultShortcutKeys(), new(viewModel.ZoomInShortcut.ModifierKeys, viewModel.ZoomInShortcut.Key));
        Assert.Equal(AppShortcutKind.ZoomOut.GetDefaultShortcutKeys(), new(viewModel.ZoomOutShortcut.ModifierKeys, viewModel.ZoomOutShortcut.Key));
        Assert.Equal(AppShortcutKind.ResetZoom.GetDefaultShortcutKeys(), new(viewModel.ResetZoomShortcut.ModifierKeys, viewModel.ResetZoomShortcut.Key));
        Assert.Equal(AppShortcutKind.ToggleTracking.GetDefaultShortcutKeys(), new(viewModel.ToggleTrackingShortcut.ModifierKeys, viewModel.ToggleTrackingShortcut.Key));
        _settingsService.Received(1).GetShortcutKeys(AppShortcutKind.StartStop);
        _settingsService.Received(1).GetShortcutKeys(AppShortcutKind.ZoomIn);
        _settingsService.Received(1).GetShortcutKeys(AppShortcutKind.ZoomOut);
        _settingsService.Received(1).GetShortcutKeys(AppShortcutKind.ResetZoom);
        _settingsService.Received(1).GetShortcutKeys(AppShortcutKind.ToggleTracking);
    }
}
