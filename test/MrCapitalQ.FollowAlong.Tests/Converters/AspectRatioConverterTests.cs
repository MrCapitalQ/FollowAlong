using MrCapitalQ.FollowAlong.Converters;

namespace MrCapitalQ.FollowAlong.Tests.Converters;

public class AspectRatioConverterTests
{
    [InlineData("0.5", "10", 5)]
    [InlineData("not parsable", "10", 0)]
    [InlineData("0.5", "not parsable", 0)]
    [InlineData("not parsable", "not parsable", 0)]
    [InlineData(null, "10", 0)]
    [InlineData("0.5", null, 0)]
    [InlineData(null, null, 0)]
    [Theory]
    public void Convert_ReturnsValueAndParameterMultiplied(object? value, object? parameter, double expected)
    {
        var actual = Convert.ToDouble(new AspectRatioConverter().Convert(value, typeof(object), parameter, string.Empty));

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ConvertBack_ThrowsNotImplementedException()
    {
        Assert.Throws<NotImplementedException>(() => new AspectRatioConverter().ConvertBack(string.Empty, typeof(object), string.Empty, string.Empty));
    }
}
