using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using MrCapitalQ.FollowAlong.Infrastructure;
using System.Diagnostics.CodeAnalysis;

namespace MrCapitalQ.FollowAlong.Controls;

[ExcludeFromCodeCoverage(Justification = ExcludeFromCoverageJustifications.RequiresUIThread)]
[ContentProperty(Name = nameof(MainContent))]
public sealed class KeyboardKey : Control
{
    public static readonly DependencyProperty MainContentProperty =
       DependencyProperty.Register(nameof(MainContent),
          typeof(string),
          typeof(KeyboardKey),
          new PropertyMetadata(default(string)));

    public KeyboardKey() => DefaultStyleKey = typeof(KeyboardKey);

    public string? MainContent
    {
        get { return GetValue(MainContentProperty)?.ToString(); }
        set { SetValue(MainContentProperty, value); }
    }
}
