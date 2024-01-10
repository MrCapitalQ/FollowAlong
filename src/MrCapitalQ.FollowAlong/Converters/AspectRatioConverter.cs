using Microsoft.UI.Xaml.Data;
using System;

namespace MrCapitalQ.FollowAlong.Converters
{
    internal class AspectRatioConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!double.TryParse(value?.ToString(), out var aspectRatio) || !double.TryParse(parameter?.ToString(), out var height))
                return 0;

            return height * aspectRatio;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
