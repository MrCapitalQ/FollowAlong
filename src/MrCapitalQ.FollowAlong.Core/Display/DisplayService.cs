using Microsoft.UI.Windowing;
using System.Collections.Generic;
using Windows.Graphics;

namespace MrCapitalQ.FollowAlong.Core.Display
{
    public class DisplayService
    {
        public IEnumerable<DisplayItem> GetAll()
        {
            // Must use for loop like this to work around a known issue that causes invalid cast exception.
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

    public record DisplayItem(bool IsPrimary, RectInt32 OuterBounds, RectInt32 WorkArea, ulong DisplayId);
}
