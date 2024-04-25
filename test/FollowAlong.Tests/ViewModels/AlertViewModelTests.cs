using Microsoft.UI.Xaml.Controls;
using MrCapitalQ.FollowAlong.ViewModels;

namespace MrCapitalQ.FollowAlong.Tests.ViewModels;

public class AlertViewModelTests
{
    [Fact]
    public void Info_MessageOnly_CreatesViewModel()
    {
        var expectedMessage = "Message";

        var actual = AlertViewModel.Info(expectedMessage);

        Assert.Equal(InfoBarSeverity.Informational, actual.Severity);
        Assert.Null(actual.Title);
        Assert.Equal(expectedMessage, actual.Message);
    }

    [Fact]
    public void Info_MessageAndTitle_CreatesViewModel()
    {
        var expectedTitle = "Title";
        var expectedMessage = "Message";

        var actual = AlertViewModel.Info(expectedTitle, expectedMessage);

        Assert.Equal(InfoBarSeverity.Informational, actual.Severity);
        Assert.Equal(expectedTitle, actual.Title);
        Assert.Equal(expectedMessage, actual.Message);
    }

    [Fact]
    public void Warning_MessageOnly_CreatesViewModel()
    {
        var expectedMessage = "Message";

        var actual = AlertViewModel.Warning(expectedMessage);

        Assert.Equal(InfoBarSeverity.Warning, actual.Severity);
        Assert.Null(actual.Title);
        Assert.Equal(expectedMessage, actual.Message);
    }

    [Fact]
    public void Warning_MessageAndTitle_CreatesViewModel()
    {
        var expectedTitle = "Title";
        var expectedMessage = "Message";

        var actual = AlertViewModel.Warning(expectedTitle, expectedMessage);

        Assert.Equal(InfoBarSeverity.Warning, actual.Severity);
        Assert.Equal(expectedTitle, actual.Title);
        Assert.Equal(expectedMessage, actual.Message);
    }

    [Fact]
    public void Error_MessageOnly_CreatesViewModel()
    {
        var expectedMessage = "Message";

        var actual = AlertViewModel.Error(expectedMessage);

        Assert.Equal(InfoBarSeverity.Error, actual.Severity);
        Assert.Null(actual.Title);
        Assert.Equal(expectedMessage, actual.Message);
    }

    [Fact]
    public void Error_MessageAndTitle_CreatesViewModel()
    {
        var expectedTitle = "Title";
        var expectedMessage = "Message";

        var actual = AlertViewModel.Error(expectedTitle, expectedMessage);

        Assert.Equal(InfoBarSeverity.Error, actual.Severity);
        Assert.Equal(expectedTitle, actual.Title);
        Assert.Equal(expectedMessage, actual.Message);
    }
}
