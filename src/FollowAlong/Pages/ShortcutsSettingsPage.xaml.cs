using Microsoft.UI.Xaml.Controls;
using MrCapitalQ.FollowAlong.Infrastructure;
using System.Diagnostics.CodeAnalysis;

namespace MrCapitalQ.FollowAlong.Pages;

[ExcludeFromCodeCoverage(Justification = ExcludeFromCoverageJustifications.RequiresUIThread)]
public sealed partial class ShortcutsSettingsPage : Page
{
    public ShortcutsSettingsPage() => InitializeComponent();
}
