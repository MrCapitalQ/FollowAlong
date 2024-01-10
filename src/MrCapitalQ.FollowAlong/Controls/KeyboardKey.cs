using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;

namespace MrCapitalQ.FollowAlong.Controls
{
    [ContentProperty(Name = nameof(MainContent))]
    public sealed class KeyboardKey : Control
    {
        public static readonly DependencyProperty MainContentProperty =
           DependencyProperty.Register(nameof(MainContent),
              typeof(string),
              typeof(KeyboardKey),
              new PropertyMetadata(default(string)));

        public KeyboardKey() => DefaultStyleKey = typeof(KeyboardKey);

        public string? MainContent
        {
            get { return GetValue(MainContentProperty)?.ToString(); }
            set { SetValue(MainContentProperty, value); }
        }
    }
}
