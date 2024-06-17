using MrCapitalQ.FollowAlong.Core.AppData;
using MrCapitalQ.FollowAlong.ViewModels;

namespace MrCapitalQ.FollowAlong.Tests.ViewModels;

public class TrackingSettingsViewModelTests
{
    private readonly ISettingsService _settingsService;

    private readonly TrackingSettingsViewModel _viewModel;

    public TrackingSettingsViewModelTests()
    {
        _settingsService = Substitute.For<ISettingsService>();

        _settingsService.GetHorizontalBoundsThreshold().Returns(0.5);
        _settingsService.GetVerticalBoundsThreshold().Returns(0.5);

        _viewModel = new(_settingsService);

        _settingsService.ClearReceivedCalls();
    }

    [Fact]
    public void Ctor_InitializesFromSettingsService()
    {
        var viewModel = new TrackingSettingsViewModel(_settingsService);

        Assert.Equal(50, viewModel.HorizontalBoundsThreshold);
        Assert.Equal(50, viewModel.VerticalBoundsThreshold);
        _settingsService.Received(1).GetHorizontalBoundsThreshold();
        _settingsService.Received(1).GetVerticalBoundsThreshold();
    }

    [Fact]
    public void SetHorizontalBoundsThreshold_UpdatesSettings()
    {
        _viewModel.HorizontalBoundsThreshold = 33;

        _settingsService.Received(1).SetHorizontalBoundsThreshold(0.33);
    }

    [Fact]
    public void SetVerticalBoundsThreshold_UpdatesSettings()
    {
        _viewModel.VerticalBoundsThreshold = 33;

        _settingsService.Received(1).SetVerticalBoundsThreshold(0.33);
    }
}
