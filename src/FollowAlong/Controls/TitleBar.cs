using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using MrCapitalQ.FollowAlong.Core;
using System.Diagnostics.CodeAnalysis;

namespace MrCapitalQ.FollowAlong.Controls;

[ExcludeFromCodeCoverage(Justification = JustificationConstants.UIThreadTestExclusionJustification)]
public sealed class TitleBar : Control
{
    public static readonly DependencyProperty IconProperty =
       DependencyProperty.Register(nameof(Icon),
          typeof(ImageSource),
          typeof(TitleBar),
          new PropertyMetadata(default(ImageSource)));
    public static readonly DependencyProperty TitleProperty =
       DependencyProperty.Register(nameof(Title),
          typeof(string),
          typeof(TitleBar),
          new PropertyMetadata(default(string)));
    public static readonly DependencyProperty WindowProperty =
       DependencyProperty.Register(nameof(Window),
          typeof(Window),
          typeof(TitleBar),
          new PropertyMetadata(default(Window)));
    public static readonly DependencyProperty LeftInsetProperty =
       DependencyProperty.Register(nameof(LeftInset),
          typeof(double),
          typeof(TitleBar),
          new PropertyMetadata(default(double)));
    public static readonly DependencyProperty RightInsetProperty =
       DependencyProperty.Register(nameof(RightInset),
          typeof(double),
          typeof(TitleBar),
          new PropertyMetadata(default(double)));
    public static readonly DependencyProperty IconVisibilityProperty =
       DependencyProperty.Register(nameof(IconVisibility),
          typeof(Visibility),
          typeof(TitleBar),
          new PropertyMetadata(Visibility.Collapsed));

    public TitleBar()
    {
        DefaultStyleKey = typeof(TitleBar);
        Loaded += TitleBar_Loaded;
    }

    public ImageSource? Icon
    {
        get { return GetValue(IconProperty) as ImageSource; }
        set
        {
            SetValue(IconProperty, value);
            IconVisibility = value is null ? Visibility.Collapsed : Visibility.Visible;
        }
    }

    public string? Title
    {
        get { return GetValue(TitleProperty)?.ToString(); }
        set { SetValue(TitleProperty, value); }
    }

    public Window? Window
    {
        get { return GetValue(WindowProperty) as Window; }
        set
        {
            SetValue(WindowProperty, value);
            UpdateTitleBarLayout();
            if (value is not null)
                value.Activated += Window_Activated;
        }
    }

    public double LeftInset
    {
        get { return GetValue(LeftInsetProperty) as double? ?? 0; }
        private set { SetValue(LeftInsetProperty, Math.Max(0, value)); }
    }

    public double RightInset
    {
        get { return GetValue(RightInsetProperty) as double? ?? 0; }
        private set { SetValue(RightInsetProperty, Math.Max(0, value)); }
    }

    public Visibility IconVisibility
    {
        get { return GetValue(IconVisibilityProperty) as Visibility? ?? Visibility.Collapsed; }
        private set { SetValue(IconVisibilityProperty, value); }
    }

    private void UpdateTitleBarLayout()
    {
        if (Window?.AppWindow?.TitleBar is null)
            return;

        var rasterizationScale = XamlRoot?.RasterizationScale ?? 1;
        Height = Window.AppWindow.TitleBar.Height / rasterizationScale;
        LeftInset = Window.AppWindow.TitleBar.LeftInset / rasterizationScale;
        RightInset = Window.AppWindow.TitleBar.RightInset / rasterizationScale;

    }

    private void TitleBar_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= TitleBar_Loaded;
        UpdateTitleBarLayout();
        XamlRoot.Changed += XamlRoot_Changed;
    }

    private void XamlRoot_Changed(XamlRoot sender, XamlRootChangedEventArgs args) => UpdateTitleBarLayout();

    private void Window_Activated(object sender, WindowActivatedEventArgs args)
    {
        if (args.WindowActivationState == WindowActivationState.Deactivated)
            Foreground = (Brush)App.Current.Resources["WindowCaptionForegroundDisabled"];
        else
            Foreground = (Brush)App.Current.Resources["WindowCaptionForeground"];
    }
}
