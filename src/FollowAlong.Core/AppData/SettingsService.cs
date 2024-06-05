using MrCapitalQ.FollowAlong.Core.Keyboard;
using System.Text.Json;

namespace MrCapitalQ.FollowAlong.Core.AppData;

public class SettingsService(IApplicationDataStore localApplicationData) : ISettingsService
{
    private const string HasBeenLaunchedOnceSettingsKey = "HasBeenLaunchedOnce";
    private const string ShortcutKeySettingsKeyTemplate = "ShortcutKeys_{0}";

    private readonly IApplicationDataStore _applicationDataStore = localApplicationData;

    public bool GetHasBeenLaunchedOnce()
        => _applicationDataStore.GetValue(HasBeenLaunchedOnceSettingsKey) is bool value && value;

    public void SetHasBeenLaunchedOnce()
        => _applicationDataStore.SetValue(HasBeenLaunchedOnceSettingsKey, true);

    public ShortcutKeys GetShortcutKeys(AppShortcutKind shortcutKind)
    {
        if (_applicationDataStore.GetValue(string.Format(ShortcutKeySettingsKeyTemplate, shortcutKind)) is string value
            && JsonSerializer.Deserialize<ShortcutKeys>(value) is ShortcutKeys shortcutKeys)
            return shortcutKeys;

        var defaultShortcutKeys = shortcutKind.GetDefaultShortcutKeys();
        SetShortcutKeys(shortcutKind, defaultShortcutKeys);
        return defaultShortcutKeys;
    }

    public void SetShortcutKeys(AppShortcutKind shortcutKind, ShortcutKeys shortcutKeys)
        => _applicationDataStore.SetValue(string.Format(ShortcutKeySettingsKeyTemplate, shortcutKind), JsonSerializer.Serialize(shortcutKeys));
}
