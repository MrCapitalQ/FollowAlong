using Microsoft.UI.Windowing;
using System.Diagnostics.CodeAnalysis;

namespace MrCapitalQ.FollowAlong.Core.Display;

[ExcludeFromCodeCoverage(Justification = "Uses native static APIs that can't be mocked.")]
public class DisplayService : IDisplayService
{
    public IEnumerable<DisplayItem> GetAll()
    {
        // Must use for loop like this to work around a known issue that causes invalid cast exception when using LINQ
        // Select().
        var displayAreas = DisplayArea.FindAll();
        for (var i = 0; i < displayAreas.Count; i++)
        {
            var displayArea = displayAreas[i];
            yield return new DisplayItem(displayArea.IsPrimary,
                displayArea.OuterBounds,
                displayArea.WorkArea,
                displayArea.DisplayId.Value);
        }
    }
}
