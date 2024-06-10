using MrCapitalQ.FollowAlong.Converters;

namespace MrCapitalQ.FollowAlong.Tests.Converters;

public class PercentageValueConverterTests
{
    private readonly PercentageValueConverter _converter = new();

    [InlineData(null, "NaN")]
    [InlineData("not a number", "NaN")]
    [InlineData("50", "50%")]
    [InlineData("50.4", "50%")]
    [InlineData("50.5", "51%")]
    [InlineData("50.6", "51%")]
    [InlineData(50, "50%")]
    [InlineData(50.4, "50%")]
    [InlineData(50.5, "51%")]
    [InlineData(50.6, "51%")]
    [Theory]
    public void Convert_ConvertsToPercentageFormattedString(object? value, string expected)
    {
        var actual = _converter.Convert(value, typeof(string), null, null);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ConvertBack_ThrowsNotImplementedException()
    {
        Assert.Throws<NotImplementedException>(() => _converter.ConvertBack("50%", typeof(string), null, null));
    }
}
