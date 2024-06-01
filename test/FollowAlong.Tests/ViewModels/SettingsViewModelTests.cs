using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml.Media.Animation;
using MrCapitalQ.FollowAlong.Infrastructure.Startup;
using MrCapitalQ.FollowAlong.Messages;
using MrCapitalQ.FollowAlong.Pages;
using MrCapitalQ.FollowAlong.Shared;
using MrCapitalQ.FollowAlong.ViewModels;
using Windows.ApplicationModel;

namespace MrCapitalQ.FollowAlong.Tests.ViewModels;

public class SettingsViewModelTests
{
    private readonly IStartupTaskService _startupTaskService;
    private readonly IPackageInfo _packageInfo;
    private readonly IMessenger _messenger;

    private readonly SettingsViewModel _viewModel;
    public SettingsViewModelTests()
    {
        _startupTaskService = Substitute.For<IStartupTaskService>();
        _packageInfo = Substitute.For<IPackageInfo>();
        _messenger = Substitute.For<IMessenger>();

        _viewModel = new(_startupTaskService, _packageInfo, _messenger);
    }

    [Fact]
    public void Ctor_InitializesWithPackageInfo()
    {
        var expectedAppDisplayName = "AppName";
        _packageInfo.DisplayName.Returns(expectedAppDisplayName);
        _packageInfo.Version.Returns(new PackageVersion(1, 2, 3, 0));

        var viewModel = new SettingsViewModel(_startupTaskService, _packageInfo, _messenger);

        Assert.Equal(expectedAppDisplayName, viewModel.AppDisplayName);
        Assert.Equal("1.2.3", viewModel.Version);
    }

    [InlineData(AppStartupState.Disabled, false, true, "Start automatically in the background when you sign in")]
    [InlineData(AppStartupState.DisabledByUser, false, false, "Startup is disabled at the system level and must be enabled using the Startup tab in Task Manager")]
    [InlineData(AppStartupState.Enabled, true, true, "Start automatically in the background when you sign in")]
    [InlineData(AppStartupState.DisabledByPolicy, false, false, "Startup is disabled by group policy or not supported on this device")]
    [InlineData(AppStartupState.EnabledByPolicy, true, false, "Startup is enabled by group policy")]
    [Theory]
    public void Ctor_InitializesStartupProperties(AppStartupState startupState,
        bool expectedIsStartupOn,
        bool expectedIsStartupToggleEnabled,
        string expectedStartupSettingsText)
    {
        _startupTaskService.GetStartupStateAsync().Returns(startupState);

        var viewModel = new SettingsViewModel(_startupTaskService, _packageInfo, _messenger);

        Assert.Equal(expectedIsStartupOn, viewModel.IsStartupOn);
        Assert.Equal(expectedIsStartupToggleEnabled, viewModel.IsStartupToggleEnabled);
        Assert.Equal(expectedStartupSettingsText, viewModel.StartupSettingsText);
    }

    [InlineData(false, AppStartupState.Disabled, false)]
    [InlineData(true, AppStartupState.Enabled, true)]
    [Theory]
    public void IsStartupOn_SetsStartupState(bool isStartOn, AppStartupState startupState, bool expectedIsStartupOn)
    {
        _startupTaskService.GetStartupStateAsync().Returns(startupState);

        _viewModel.IsStartupOn = isStartOn;

        Assert.Equal(expectedIsStartupOn, _viewModel.IsStartupOn);
        _startupTaskService.Received(1).SetStartupStateAsync(isStartOn);
    }

    [Fact]
    public void ShortcutsSettingsCommand_SendsNavigateMessage()
    {
        var navigateMessage = new SlideNavigateMessage(typeof(ShortcutsSettingsPage), SlideNavigationTransitionEffect.FromRight);

        _viewModel.ShortcutsSettingsCommand.Execute(null);

        _messenger.Received(1).Send<NavigateMessage, TestMessengerToken>(navigateMessage, Arg.Any<TestMessengerToken>());
    }
}
