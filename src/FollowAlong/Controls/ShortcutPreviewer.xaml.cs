using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MrCapitalQ.FollowAlong.Infrastructure;
using System.Diagnostics.CodeAnalysis;
using Windows.System;

namespace MrCapitalQ.FollowAlong.Controls;

[ExcludeFromCodeCoverage(Justification = ExcludeFromCoverageJustifications.RequiresUIThread)]
public sealed partial class ShortcutPreviewer : UserControl
{
    public static readonly DependencyProperty ShortcutKeysProperty =
       DependencyProperty.Register(nameof(ShortcutKeys),
          typeof(IEnumerable<VirtualKey>),
          typeof(KeyboardKey),
          new PropertyMetadata(Enumerable.Empty<VirtualKey>()));
    public static readonly DependencyProperty TransientShortcutKeysProperty =
       DependencyProperty.Register(nameof(TransientShortcutKeys),
          typeof(IEnumerable<VirtualKey>),
          typeof(KeyboardKey),
          new PropertyMetadata(Enumerable.Empty<VirtualKey>()));

    public ShortcutPreviewer() => InitializeComponent();

    public IEnumerable<VirtualKey> ShortcutKeys
    {
        get => (IEnumerable<VirtualKey>)GetValue(ShortcutKeysProperty);
        set => SetValue(ShortcutKeysProperty, value);
    }

    public IEnumerable<VirtualKey> TransientShortcutKeys
    {
        get => (IEnumerable<VirtualKey>)GetValue(TransientShortcutKeysProperty);
        set => SetValue(TransientShortcutKeysProperty, value);
    }

    private void ClearButton_Click(object sender, RoutedEventArgs e) => ShortcutKeys = [];
}
