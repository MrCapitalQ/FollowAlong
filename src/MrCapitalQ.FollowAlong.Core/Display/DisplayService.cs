using Microsoft.UI.Windowing;
using System.Collections.Generic;

namespace MrCapitalQ.FollowAlong.Core.Display
{
    public class DisplayService
    {
        public IEnumerable<DisplayArea> GetAll()
        {
            // Must use for loop like this to work around a known issue that causes invalid cast exception.
            var displayAreas = DisplayArea.FindAll();
            for (var i = 0; i < displayAreas.Count; i++)
            {
                yield return displayAreas[i];
            }
        }
    }
}
