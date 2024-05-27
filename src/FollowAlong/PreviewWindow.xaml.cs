using Microsoft.UI.Windowing;
using MrCapitalQ.FollowAlong.Infrastructure;
using MrCapitalQ.FollowAlong.Infrastructure.Display;
using MrCapitalQ.FollowAlong.Infrastructure.Utils;
using System.Diagnostics.CodeAnalysis;
using Windows.Graphics;

namespace MrCapitalQ.FollowAlong;

[ExcludeFromCodeCoverage(Justification = ExcludeFromCoverageJustifications.RequiresUIThread)]
public sealed partial class PreviewWindow : WindowBase
{
    public PreviewWindow()
    {
        InitializeComponent();
        InitializeWindow();
    }

    private void InitializeWindow()
    {
        // Teams does not list windows with no title. Set no title so the preview window cannot be selected.
        Title = null!;

        ExtendsContentIntoTitleBar = true;
        this.SetIsExcludedFromCapture(true);

        // Move to bottom left corner of the screen.
        var displayItem = this.GetCurrentDisplayItem();
        if (displayItem is not null)
            AppWindow.Move(new PointInt32(displayItem.WorkArea.X,
                displayItem.WorkArea.Y + displayItem.WorkArea.Height - AppWindow.Size.Height));
    }
}
