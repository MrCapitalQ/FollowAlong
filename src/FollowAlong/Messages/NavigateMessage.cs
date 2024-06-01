using Microsoft.UI.Xaml.Media.Animation;

namespace MrCapitalQ.FollowAlong.Messages;

internal record NavigateMessage(Type SourcePageType, object? Parameter = null);

internal record EntranceNavigateMessage(Type SourcePageType, object? Parameter = null)
    : NavigateMessage(SourcePageType, Parameter);

internal record SlideNavigateMessage(Type SourcePageType,
    SlideNavigationTransitionEffect SlideEffect,
    object? Parameter = null)
    : NavigateMessage(SourcePageType, Parameter);
