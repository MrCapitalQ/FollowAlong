﻿using MrCapitalQ.FollowAlong.Core.Keyboard;
using System.Text.Json;

namespace MrCapitalQ.FollowAlong.Core.AppData;

public class SettingsService(IApplicationDataStore localApplicationData) : ISettingsService
{
    private const string HasBeenLaunchedOnceSettingsKey = "HasBeenLaunchedOnce";
    private const string ShortcutKeySettingsKeyTemplate = "ShortcutKeys_{0}";
    private const string HorizontalBoundsThresholdSettingsKey = "HorizontalBoundsThreshold";
    private const string VerticalBoundsThresholdSettingsKey = "VerticalBoundsThreshold";
    private const string DefaultZoomSettingsKey = "DefaultZoom";
    private const double BoundsThresholdDefault = 0.5;
    private const double DefaultZoomDefault = 1.5;

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

    public double GetHorizontalBoundsThreshold()
    {
        if (_applicationDataStore.GetValue(HorizontalBoundsThresholdSettingsKey) is double value)
            return value;

        SetHorizontalBoundsThreshold(BoundsThresholdDefault);
        return BoundsThresholdDefault;
    }

    public void SetHorizontalBoundsThreshold(double threshold)
        => _applicationDataStore.SetValue(HorizontalBoundsThresholdSettingsKey, threshold);

    public double GetVerticalBoundsThreshold()
    {
        if (_applicationDataStore.GetValue(VerticalBoundsThresholdSettingsKey) is double value)
            return value;

        SetVerticalBoundsThreshold(BoundsThresholdDefault);
        return BoundsThresholdDefault;
    }

    public void SetVerticalBoundsThreshold(double threshold)
        => _applicationDataStore.SetValue(VerticalBoundsThresholdSettingsKey, threshold);

    public double GetDefaultZoom()
    {
        if (_applicationDataStore.GetValue(DefaultZoomSettingsKey) is double value)
            return value;

        SetDefaultZoom(DefaultZoomDefault);
        return DefaultZoomDefault;
    }

    public void SetDefaultZoom(double zoom)
        => _applicationDataStore.SetValue(DefaultZoomSettingsKey, zoom);
}
