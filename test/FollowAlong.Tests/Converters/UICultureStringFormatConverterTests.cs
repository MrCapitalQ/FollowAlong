using MrCapitalQ.FollowAlong.Converters;
using System.Globalization;

namespace MrCapitalQ.FollowAlong.Tests.Converters;

public class UICultureStringFormatConverterTests
{
    private readonly UICultureStringFormatConverter _converter = new();

    [InlineData("en-US", "50.00%")]
    [InlineData("hr-HR", "50,000 %")]
    [Theory]
    public void Convert_LanguageNotProvided_ConvertsUsingCurrentUICulture(string currentUILanguage, string expected)
    {
        var value = 0.5;
        CultureInfo.CurrentUICulture = new CultureInfo(currentUILanguage);

        var actual = _converter.Convert(value, typeof(string), "{0:P}", string.Empty);

        Assert.Equal(expected, actual);
    }

    [InlineData("en-US", "50.00%")]
    [InlineData("hr-HR", "50.00%")]
    [Theory]
    public void Convert_LanguageProvided_ConvertsUsingLanguageProvided(string currentUILanguage, string expected)
    {
        var value = 0.5;
        CultureInfo.CurrentUICulture = new CultureInfo(currentUILanguage);

        var actual = _converter.Convert(value, typeof(string), "{0:P}", "en-US");

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ConvertBack_ThrowsNotImplementedException()
    {
        Assert.Throws<NotImplementedException>(() => _converter.ConvertBack("50%", typeof(string), new object(), string.Empty));
    }
}
