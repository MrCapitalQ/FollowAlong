using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Hosting;
using MrCapitalQ.FollowAlong.Infrastructure;
using MrCapitalQ.FollowAlong.ViewModels;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace MrCapitalQ.FollowAlong.Pages;

[ExcludeFromCodeCoverage(Justification = ExcludeFromCoverageJustifications.RequiresUIThread)]
public sealed partial class TrackingSettingsPage : Page
{
    private readonly TrackingSettingsViewModel _viewModel;

    public TrackingSettingsPage()
    {
        InitializeComponent();
        _viewModel = App.Current.Services.GetRequiredService<TrackingSettingsViewModel>();
        _viewModel.PropertyChanged += ViewModel_PropertyChanged;

        CreatePointerAnimation();
        CreateDeadZoneAnimation();
    }

    private void CreatePointerAnimation()
    {
        var compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
        var stepEasingFunction = compositor.CreateStepEasingFunction();

        var animation = compositor.CreateVector3KeyFrameAnimation();
        animation.Target = "Translation";
        animation.IterationBehavior = AnimationIterationBehavior.Forever;
        animation.Duration = TimeSpan.FromSeconds(10);

        animation.InsertKeyFrame(0.25f, new Vector3(175, 0, 0));
        animation.InsertKeyFrame(0.251f, new Vector3(0), stepEasingFunction);

        animation.InsertKeyFrame(0.5f, new Vector3(-175, 0, 0));
        animation.InsertKeyFrame(0.501f, new Vector3(0), stepEasingFunction);

        animation.InsertKeyFrame(0.75f, new Vector3(0, -100, 0));
        animation.InsertKeyFrame(0.751f, new Vector3(0), stepEasingFunction);

        animation.InsertKeyFrame(1, new Vector3(0, 100, 0));
        Pointer.StartAnimation(animation);
    }

    private void CreateDeadZoneAnimation()
    {
        var compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;

        var animation = compositor.CreateExpressionAnimation();
        animation.Target = "Translation";

        const string stepperX = "Clamp(Floor(Abs(pointer.Translation.X)/ (thresholdBounds.ActualSize.X / 2)), 0, 1)";
        const string stepperY = "Clamp(Floor(Abs(pointer.Translation.Y)/ (thresholdBounds.ActualSize.Y / 2)), 0, 1)";
        const string translationMagnitudeX = "((thresholdBounds.ActualSize.X / 2) - Abs(pointer.Translation.X))";
        const string translationMagnitudeY = "((thresholdBounds.ActualSize.Y / 2) - Abs(pointer.Translation.Y))";
        const string signModifierX = "Clamp(-pointer.Translation.X, -1, 1)";
        const string signModifierY = "Clamp(-pointer.Translation.Y, -1, 1)";
        const string translationLimitX = "((screenArea.actualSize.X - captureZone.actualSize.X) / 2)";
        const string translationLimitY = "((screenArea.actualSize.Y - captureZone.actualSize.Y) / 2)";
        const string expressionX = $"Clamp({stepperX} * {translationMagnitudeX} * {signModifierX}, -{translationLimitX}, {translationLimitX})";
        const string expressionY = $"Clamp({stepperY} * {translationMagnitudeY} * {signModifierY}, -{translationLimitY}, {translationLimitY})";

        animation.Expression = $"Vector3({expressionX}, {expressionY}, 0)";
        animation.SetExpressionReferenceParameter("pointer", Pointer);
        animation.SetExpressionReferenceParameter("thresholdBounds", ThresholdBounds);
        animation.SetExpressionReferenceParameter("screenArea", ScreenArea);
        animation.SetExpressionReferenceParameter("captureZone", CaptureZone);

        CaptureZoneContainer.StartAnimation(animation);
    }

    private void UpdateThresholdBounds()
    {
        var minHeight = ThresholdBounds.BorderThickness.Top + ThresholdBounds.BorderThickness.Bottom;
        var minWidth = ThresholdBounds.BorderThickness.Left + ThresholdBounds.BorderThickness.Right;
        ThresholdBounds.Width = Math.Max(minHeight, CaptureZone.ActualWidth - (CaptureZone.ActualWidth * _viewModel.HorizontalBoundsThreshold / 100));
        ThresholdBounds.Height = Math.Max(minWidth, CaptureZone.ActualHeight - (CaptureZone.ActualHeight * _viewModel.VerticalBoundsThreshold / 100));
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(TrackingSettingsViewModel.HorizontalBoundsThreshold)
            or nameof(TrackingSettingsViewModel.VerticalBoundsThreshold))
            UpdateThresholdBounds();
    }

    private void CaptureZone_SizeChanged(object sender, SizeChangedEventArgs e) => UpdateThresholdBounds();
}
