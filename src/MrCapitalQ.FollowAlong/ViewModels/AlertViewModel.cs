using Microsoft.UI.Xaml.Controls;

namespace MrCapitalQ.FollowAlong.ViewModels
{
    internal record AlertViewModel
    {
        private AlertViewModel(InfoBarSeverity severity, string? title, string message)
        {
            Severity = severity;
            Title = title;
            Message = message;
        }

        public InfoBarSeverity Severity { get; }
        public string? Title { get; }
        public string Message { get; }

        public static AlertViewModel Info(string message) => new(InfoBarSeverity.Informational, null, message);
        public static AlertViewModel Info(string title, string message) => new(InfoBarSeverity.Informational, title, message);
        public static AlertViewModel Warning(string message) => new(InfoBarSeverity.Warning, null, message);
        public static AlertViewModel Warning(string title, string message) => new(InfoBarSeverity.Warning, title, message);
        public static AlertViewModel Error(string message) => new(InfoBarSeverity.Error, null, message);
        public static AlertViewModel Error(string title, string message) => new(InfoBarSeverity.Error, title, message);
    };
}
