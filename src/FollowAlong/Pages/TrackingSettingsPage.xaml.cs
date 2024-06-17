using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using MrCapitalQ.FollowAlong.Infrastructure;
using MrCapitalQ.FollowAlong.ViewModels;
using System.Diagnostics.CodeAnalysis;

namespace MrCapitalQ.FollowAlong.Pages;

[ExcludeFromCodeCoverage(Justification = ExcludeFromCoverageJustifications.RequiresUIThread)]
public sealed partial class TrackingSettingsPage : Page
{
    private readonly TrackingSettingsViewModel _viewModel;

    public TrackingSettingsPage()
    {
        InitializeComponent();
        _viewModel = App.Current.Services.GetRequiredService<TrackingSettingsViewModel>();
    }
}
