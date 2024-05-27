using Windows.Graphics;

namespace MrCapitalQ.FollowAlong.Infrastructure.Display;

public record DisplayItem(bool IsPrimary, RectInt32 OuterBounds, RectInt32 WorkArea, ulong DisplayId);
