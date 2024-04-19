using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Diagnostics.CodeAnalysis;

namespace MrCapitalQ.FollowAlong.Converters;

[ExcludeFromCodeCoverage(Justification = "Creates BitmapImage that won't work during test execution.")]
internal class StreamToImageConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, string language)
    {
        if (value is not Stream stream)
            return null;

        var image = new BitmapImage();
        image.SetSource(stream.AsRandomAccessStream());

        return image;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, string language)
        => throw new NotImplementedException();
}
