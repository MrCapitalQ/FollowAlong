using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using MrCapitalQ.FollowAlong.Controls;
using MrCapitalQ.FollowAlong.Core.Keyboard;
using MrCapitalQ.FollowAlong.Infrastructure;
using MrCapitalQ.FollowAlong.Infrastructure.Utils;
using MrCapitalQ.FollowAlong.Messages;
using MrCapitalQ.FollowAlong.Pages;
using MrCapitalQ.FollowAlong.Shared;
using System.Diagnostics.CodeAnalysis;
using Windows.ApplicationModel;
using Windows.System;
using Windows.UI.Core;

namespace MrCapitalQ.FollowAlong;

[ExcludeFromCodeCoverage(Justification = ExcludeFromCoverageJustifications.RequiresUIThread)]
public sealed partial class MainWindow : WindowBase
{
    private readonly IMessenger _messenger;
    private readonly ShortcutPreviewer _shortcutPreviewer;
    private readonly ContentDialog _editShortcutDialog;

    private bool _shouldPreviewPressedKeys = true;

    public MainWindow(IMessenger messenger)
    {
        InitializeComponent();

        _messenger = messenger;

        Title = Package.Current.GetAppListEntries()[0].DisplayInfo.DisplayName;
        ExtendsContentIntoTitleBar = true;
        PersistenceId = nameof(MainWindow);
        this.SetIsExcludedFromCapture(false);
        Closed += MainWindow_Closed;
        Activated += MainWindow_Activated;

        RootFrame.Navigated += RootFrame_Navigated;
        RootFrame.Navigate(typeof(MainPage));

        _shortcutPreviewer = new();
        _editShortcutDialog = new()
        {
            Title = "Change shortcut",
            Content = _shortcutPreviewer,
            PrimaryButtonText = "Save",
            SecondaryButtonText = "Reset",
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Primary,
            Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style
        };
        _editShortcutDialog.PreviewKeyDown += Dialog_PreviewKeyDown;
        _editShortcutDialog.PreviewKeyUp += Dialog_PreviewKeyUp;

        _messenger.Register<MainWindow, NavigateMessage>(this, (r, m) =>
        {
            NavigationTransitionInfo? transitionInfo = m switch
            {
                EntranceNavigateMessage entranceNavigateMessage => new EntranceNavigationTransitionInfo(),
                SlideNavigateMessage slideNavigateMessage => new SlideNavigationTransitionInfo { Effect = slideNavigateMessage.SlideEffect },
                _ => null
            };
            r.RootFrame.Navigate(m.SourcePageType, m.Parameter, transitionInfo);
        });

        _messenger.Register<MainWindow, ShowChangeShortcutDialogMessage>(this, async (r, m) =>
        {
            r._shortcutPreviewer.ShortcutKeys = m.CurrentShortcutKeys.ToVirtualKeys();
            r._shortcutPreviewer.TransientShortcutKeys = [];
            r._editShortcutDialog.XamlRoot = r.Content.XamlRoot;

            r._messenger.Send(UnregisterShortcutsMessage.Instance);

            var result = await r._editShortcutDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
                r._messenger.Send(new ShortcutChangedMessage(m.ShortcutKind, r._shortcutPreviewer.ShortcutKeys.ToShortcutKeys()));
            else if (result == ContentDialogResult.Secondary)
                r._messenger.Send(new ShortcutChangedMessage(m.ShortcutKind, m.ShortcutKind.GetDefaultShortcutKeys()));

            r._messenger.Send(RegisterShortcutsMessage.Instance);
        });
    }

    private static List<VirtualKey> GetPressedModifierKeys()
    {
        var pressedKeys = new List<VirtualKey>();

        if (InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.LeftWindows).HasFlag(CoreVirtualKeyStates.Down)
            || InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.RightWindows).HasFlag(CoreVirtualKeyStates.Down))
            pressedKeys.Add(VirtualKey.LeftWindows);

        if (InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Control).HasFlag(CoreVirtualKeyStates.Down))
            pressedKeys.Add(VirtualKey.Control);

        if (InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Shift).HasFlag(CoreVirtualKeyStates.Down))
            pressedKeys.Add(VirtualKey.Shift);

        if (InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Menu).HasFlag(CoreVirtualKeyStates.Down))
            pressedKeys.Add(VirtualKey.Menu);

        return pressedKeys;
    }

    private void GoBack()
    {
        if (RootFrame.CanGoBack)
            RootFrame.GoBack();
    }

    private void GoForward()
    {
        if (RootFrame.CanGoForward)
            RootFrame.GoForward();
    }

    private void MainWindow_Closed(object sender, WindowEventArgs args)
    {
        Closed -= MainWindow_Closed;
        Activated -= MainWindow_Activated;
        RootFrame.Navigated -= RootFrame_Navigated;
        _editShortcutDialog.PreviewKeyDown -= Dialog_PreviewKeyDown;
        _editShortcutDialog.PreviewKeyUp -= Dialog_PreviewKeyUp;
        _messenger.Send(RegisterShortcutsMessage.Instance);
        _messenger.UnregisterAll(this);
    }

    private void TitleBar_BackRequested(object sender, EventArgs e) => GoBack();

    private void Grid_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        var pointerProperties = e.GetCurrentPoint(sender as UIElement).Properties;
        if (pointerProperties.IsXButton1Pressed)
        {
            GoBack();
            e.Handled = true;
        }
        else if (pointerProperties.IsXButton2Pressed)
        {
            GoForward();
            e.Handled = true;
        }
    }

    private void BackKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
    {
        GoBack();
        args.Handled = true;
    }

    private void ForwardKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
    {
        GoForward();
        args.Handled = true;
    }

    private void RootFrame_Navigated(object sender, NavigationEventArgs e)
        => TitleBar.IsBackButtonVisible = RootFrame.CanGoBack;

    private void Dialog_PreviewKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (!_shouldPreviewPressedKeys)
            return;

        var pressedKeys = GetPressedModifierKeys();
        var isModifierKeyPressed = pressedKeys.Count > 0;

        if (isModifierKeyPressed &&
            e.Key is not (VirtualKey.LeftWindows
            or VirtualKey.RightWindows
            or VirtualKey.Control
            or VirtualKey.Shift
            or VirtualKey.Menu))
        {
            pressedKeys.Add(e.Key);

            e.Handled = true;
            _shortcutPreviewer.ShortcutKeys = pressedKeys;
            _shortcutPreviewer.TransientShortcutKeys = [];
            _shouldPreviewPressedKeys = false;
            return;
        }

        _shortcutPreviewer.TransientShortcutKeys = pressedKeys;
    }

    private void Dialog_PreviewKeyUp(object sender, KeyRoutedEventArgs e)
    {
        var pressedKeys = GetPressedModifierKeys();
        if (pressedKeys.Count == 0)
            _shouldPreviewPressedKeys = true;

        if (_shouldPreviewPressedKeys)
            _shortcutPreviewer.TransientShortcutKeys = pressedKeys;
    }

    private void MainWindow_Activated(object sender, Microsoft.UI.Xaml.WindowActivatedEventArgs args)
    {
        if (args.WindowActivationState == WindowActivationState.Deactivated)
            _shortcutPreviewer.TransientShortcutKeys = [];
    }
}
