using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using MrCapitalQ.FollowAlong.Core.Tracking;
using MrCapitalQ.FollowAlong.Infrastructure;
using MrCapitalQ.FollowAlong.Infrastructure.Capture;
using MrCapitalQ.FollowAlong.Messages;
using MrCapitalQ.FollowAlong.ViewModels;
using System.Diagnostics.CodeAnalysis;

namespace MrCapitalQ.FollowAlong.Pages;

[ExcludeFromCodeCoverage(Justification = ExcludeFromCoverageJustifications.RequiresUIThread)]
public sealed partial class PreviewPage : Page
{
    private readonly PreviewViewModel _viewModel;
    private readonly TrackingTransformService _trackingTransformService;

    public PreviewPage()
    {
        InitializeComponent();

        App.Current.Services.GetRequiredService<IBitmapCaptureService>().RegisterFrameHandler(Preview);

        _trackingTransformService = App.Current.Services.GetRequiredService<TrackingTransformService>();
        _trackingTransformService.StartTrackingTransforms(Preview);

        var messenger = App.Current.Services.GetRequiredService<IMessenger>();
        messenger.Register<PreviewPage, ZoomChanged>(this, (r, m) => r._trackingTransformService.Zoom = m.Zoom);
        messenger.Register<PreviewPage, TrackingToggled>(this,
            (r, m) => r._trackingTransformService.IsTrackingEnabled = m.IsEnabled);

        _viewModel = App.Current.Services.GetRequiredService<PreviewViewModel>();
    }
}
