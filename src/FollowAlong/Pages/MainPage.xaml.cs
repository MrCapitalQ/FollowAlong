using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using MrCapitalQ.FollowAlong.Infrastructure;
using MrCapitalQ.FollowAlong.ViewModels;
using System.Diagnostics.CodeAnalysis;

namespace MrCapitalQ.FollowAlong.Pages;

[ExcludeFromCodeCoverage(Justification = ExcludeFromCoverageJustifications.RequiresUIThread)]
internal sealed partial class MainPage : Page
{
    private readonly MainViewModel _viewModel;

    public MainPage()
    {
        InitializeComponent();
        _viewModel = App.Current.Services.GetRequiredService<MainViewModel>();
        _viewModel.Load();
    }
}
