using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using MrCapitalQ.FollowAlong.Infrastructure;
using System.Diagnostics.CodeAnalysis;
using Windows.System;

namespace MrCapitalQ.FollowAlong.Controls;

[ExcludeFromCodeCoverage(Justification = ExcludeFromCoverageJustifications.RequiresUIThread)]
[ContentProperty(Name = nameof(Content))]
public sealed class KeyboardKey : Control
{
    public static readonly DependencyProperty KeyProperty =
       DependencyProperty.Register(nameof(Key),
          typeof(object),
          typeof(KeyboardKey),
          new PropertyMetadata(default));
    public static readonly DependencyProperty ContentProperty =
       DependencyProperty.Register(nameof(Content),
          typeof(string),
          typeof(KeyboardKey),
          new PropertyMetadata(default));

    private UIElement? _keyContentPresenter;
    private UIElement? _glyphKeyPresenter;
    private UIElement? _winKeyPresenter;

    public KeyboardKey() => DefaultStyleKey = typeof(KeyboardKey);

    public object? Key
    {
        get => GetValue(KeyProperty);
        set
        {
            SetValue(KeyProperty, value);
            UpdateContent();
        }
    }

    public string? Content
    {
        get => GetValue(ContentProperty)?.ToString();
        private set => SetValue(ContentProperty, value);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _keyContentPresenter = GetTemplateChild("KeyContentPresenter") as UIElement;
        _glyphKeyPresenter = GetTemplateChild("GlyphKeyPresenter") as UIElement;
        _winKeyPresenter = GetTemplateChild("WinKeyPresenter") as UIElement;

        UpdateContent();
    }

    private void UpdateContent()
    {
        (string? Content, KeyDisplayMode DisplayMode) keyDisplayInfo = Key switch
        {
            "Win" => ("Win", KeyDisplayMode.WinKey),
            string key => (key, KeyDisplayMode.Default),
            VirtualKey key => GetVirtualKeyDisplayValue(key),
            _ => (Key?.ToString(), KeyDisplayMode.Default)
        };

        Content = keyDisplayInfo.Content;

        var keyContentPresenterVisibility = Visibility.Collapsed;
        var glyphKeyPresenterVisibility = Visibility.Collapsed;
        var winKeyPresenterVisibility = Visibility.Collapsed;

        if (keyDisplayInfo.DisplayMode == KeyDisplayMode.WinKey)
            winKeyPresenterVisibility = Visibility.Visible;
        else if (keyDisplayInfo.DisplayMode == KeyDisplayMode.Glyph)
            glyphKeyPresenterVisibility = Visibility.Visible;
        else
            keyContentPresenterVisibility = Visibility.Visible;

        if (_keyContentPresenter is not null)
            _keyContentPresenter.Visibility = keyContentPresenterVisibility;

        if (_glyphKeyPresenter is not null)
            _glyphKeyPresenter.Visibility = glyphKeyPresenterVisibility;

        if (_winKeyPresenter is not null)
            _winKeyPresenter.Visibility = winKeyPresenterVisibility;
    }

    private static (string? Content, KeyDisplayMode DisplayMode) GetVirtualKeyDisplayValue(VirtualKey virtualKey)
    {
        var content = virtualKey switch
        {
            VirtualKey.LeftWindows or VirtualKey.RightWindows => "Win",
            VirtualKey.Control or VirtualKey.LeftControl or VirtualKey.RightControl => "Ctrl",
            VirtualKey.Shift or VirtualKey.LeftShift or VirtualKey.RightShift => "Shift",
            VirtualKey.Menu or VirtualKey.LeftMenu or VirtualKey.RightMenu => "Alt",
            VirtualKey.Back => "Backspace",
            VirtualKey.CapitalLock => "Caps",
            VirtualKey.Escape => "Esc",
            VirtualKey.PageUp => "PgUp",
            VirtualKey.PageDown => "PgDn",
            VirtualKey.Left => "\uF0B0",
            VirtualKey.Up => "\uF0AD",
            VirtualKey.Right => "\uF0AF",
            VirtualKey.Down => "\uF0AE",
            VirtualKey.Delete => "Del",
            VirtualKey.Number0 or VirtualKey.NumberPad0 => "0",
            VirtualKey.Number1 or VirtualKey.NumberPad1 => "1",
            VirtualKey.Number2 or VirtualKey.NumberPad2 => "2",
            VirtualKey.Number3 or VirtualKey.NumberPad3 => "3",
            VirtualKey.Number4 or VirtualKey.NumberPad4 => "4",
            VirtualKey.Number5 or VirtualKey.NumberPad5 => "5",
            VirtualKey.Number6 or VirtualKey.NumberPad6 => "6",
            VirtualKey.Number7 or VirtualKey.NumberPad7 => "7",
            VirtualKey.Number8 or VirtualKey.NumberPad8 => "8",
            VirtualKey.Number9 or VirtualKey.NumberPad9 => "9",
            (VirtualKey)186 => ";",
            (VirtualKey)187 => "\uF8AA", // +
            (VirtualKey)188 => ",",
            (VirtualKey)189 => "\uF8AB", // -
            (VirtualKey)190 => ".",
            (VirtualKey)191 => "/",
            (VirtualKey)192 => "`",
            (VirtualKey)219 => "[",
            (VirtualKey)220 => "\\",
            (VirtualKey)221 => "]",
            (VirtualKey)222 => "'",
            _ => virtualKey.ToString()
        };

        var displayMode = virtualKey switch
        {
            VirtualKey.LeftWindows or VirtualKey.RightWindows => KeyDisplayMode.WinKey,
            VirtualKey.Left or VirtualKey.Up or VirtualKey.Right or VirtualKey.Down => KeyDisplayMode.Glyph,
            (VirtualKey)187 or (VirtualKey)189 => KeyDisplayMode.Glyph, // + and -
            _ => KeyDisplayMode.Default
        };

        return (content, displayMode);
    }

    private enum KeyDisplayMode
    {
        Default,
        Glyph,
        WinKey
    }
}
