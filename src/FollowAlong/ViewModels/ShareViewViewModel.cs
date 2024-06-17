using CommunityToolkit.Mvvm.ComponentModel;
using MrCapitalQ.FollowAlong.Core.AppData;

namespace MrCapitalQ.FollowAlong.ViewModels;

internal partial class TrackingSettingsViewModel : ObservableObject
{
    private readonly ISettingsService _settingsService;

    [ObservableProperty]
    private int _horizontalBoundsThreshold = 1;

    [ObservableProperty]
    private int _verticalBoundsThreshold = 1;

    public TrackingSettingsViewModel(ISettingsService settingsService)
    {
        _settingsService = settingsService;

        HorizontalBoundsThreshold = (int)(_settingsService.GetHorizontalBoundsThreshold() * 100);
        VerticalBoundsThreshold = (int)(_settingsService.GetVerticalBoundsThreshold() * 100);
    }

    partial void OnHorizontalBoundsThresholdChanged(int value) => _settingsService.SetHorizontalBoundsThreshold(value / 100d);

    partial void OnVerticalBoundsThresholdChanged(int value) => _settingsService.SetVerticalBoundsThreshold(value / 100d);
}
