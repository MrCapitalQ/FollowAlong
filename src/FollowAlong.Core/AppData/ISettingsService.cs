using MrCapitalQ.FollowAlong.Core.Keyboard;

namespace MrCapitalQ.FollowAlong.Core.AppData;

public interface ISettingsService
{
    bool GetHasBeenLaunchedOnce();
    void SetHasBeenLaunchedOnce();

    ShortcutKeys GetShortcutKeys(AppShortcutKind shortcutKind);
    void SetShortcutKeys(AppShortcutKind shortcutKind, ShortcutKeys shortcutKeys);

    double GetHorizontalBoundsThreshold();
    void SetHorizontalBoundsThreshold(double threshold);

    double GetVerticalBoundsThreshold();
    void SetVerticalBoundsThreshold(double threshold);

    double GetZoomDefaultLevel();
    void SetZoomDefaultLevel(double zoom);

    double GetZoomStepSize();
    void SetZoomStepSize(double stepSize);
}