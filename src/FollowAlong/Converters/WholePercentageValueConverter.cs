using Microsoft.UI.Xaml.Data;
using System.Globalization;

namespace MrCapitalQ.FollowAlong.Converters;

public class WholePercentageValueConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, string? language)
    {
        var wholePercentValue = double.TryParse(value?.ToString(), out var d) ? d : double.NaN;

        return string.Format(CultureInfo.CurrentUICulture, "{0:P0}", wholePercentValue / 100d);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, string? language)
        => throw new NotImplementedException();
}
