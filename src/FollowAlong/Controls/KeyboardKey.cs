using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using MrCapitalQ.FollowAlong.Infrastructure;
using System.Diagnostics.CodeAnalysis;

namespace MrCapitalQ.FollowAlong.Controls;

[ExcludeFromCodeCoverage(Justification = ExcludeFromCoverageJustifications.RequiresUIThread)]
[ContentProperty(Name = nameof(Content))]
public sealed class KeyboardKey : Control
{
    public static readonly DependencyProperty ContentProperty =
       DependencyProperty.Register(nameof(Content),
          typeof(string),
          typeof(KeyboardKey),
          new PropertyMetadata(default));

    public KeyboardKey() => DefaultStyleKey = typeof(KeyboardKey);

    public string? Content
    {
        get => GetValue(ContentProperty)?.ToString();
        set => SetValue(ContentProperty, value);
    }
}
