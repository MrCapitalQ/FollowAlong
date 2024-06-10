using Microsoft.UI.Xaml.Data;

namespace MrCapitalQ.FollowAlong.Converters
{
    public class PercentageValueConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, string? language)
        {
            if (double.TryParse(value?.ToString(), out var d))
                return string.Format("{0:P0}", d / 100d);

            return "NaN";
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, string? language)
            => throw new NotImplementedException();
    }
}
