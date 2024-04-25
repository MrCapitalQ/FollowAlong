using Windows.Graphics;

namespace MrCapitalQ.FollowAlong.Core.Display;

public record DisplayItem(bool IsPrimary, RectInt32 OuterBounds, RectInt32 WorkArea, ulong DisplayId);
