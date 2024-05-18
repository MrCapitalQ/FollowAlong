namespace MrCapitalQ.FollowAlong.Messages;

internal record NavigateMessage(Type SourcePageType, object? Parameter = null);

internal record EntranceNavigateMessage(Type SourcePageType, object? Parameter = null)
    : NavigateMessage(SourcePageType, Parameter);
