using System.Drawing;

namespace MrCapitalQ.FollowAlong.Core.Display;

public record DisplayItem(bool IsPrimary, Rectangle OuterBounds, Rectangle WorkArea, ulong DisplayId);
