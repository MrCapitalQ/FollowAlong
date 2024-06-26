using CommunityToolkit.WinUI.Converters;
using Microsoft.UI.Xaml.Data;
using System.Globalization;

namespace MrCapitalQ.FollowAlong.Converters;

internal class UICultureStringFormatConverter : IValueConverter
{
    private readonly StringFormatConverter _innerConverter = new();

    public object? Convert(object value, Type targetType, object parameter, string language)
    {
        language = string.IsNullOrEmpty(language) ? CultureInfo.CurrentUICulture.IetfLanguageTag : language;
        return _innerConverter.Convert(value, targetType, parameter, language);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => _innerConverter.ConvertBack(value, targetType, parameter, language);
}
