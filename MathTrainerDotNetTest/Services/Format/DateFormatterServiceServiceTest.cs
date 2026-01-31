using System.Globalization;
using MathTrainerDotNet.Services.Format;

namespace MathTrainerDotNetTest.Services.Format;

public class DateFormatterServiceServiceTest
{
    private readonly IDateFormatterService service = new DateFormatterServiceService();

    [Fact]
    public void Format_GermanCulture_ShouldReturnGermanLongDate()
    {
        // Arrange
        var date = new DateTime(2024, 5, 1);
        var culture = new CultureInfo("de-DE");

        // Act
        var result = this.service.Format(date, culture);

        // Assert
        Assert.Equal("Mittwoch, 1. Mai 2024", result);
    }

    [Fact]
    public void Format_EnglishCulture_ShouldReturnEnglishLongDate()
    {
        // Arrange
        var date = new DateTime(2024, 5, 1);
        var culture = new CultureInfo("en-US");

        // Act
        var result = this.service.Format(date, culture);

        // Assert
        Assert.Equal("Wednesday, May 1, 2024", result);
    }

    [Fact]
    public void Format_NullCulture_ShouldThrowArgumentNullException()
    {
        // Arrange
        var date = DateTime.Now;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => this.service.Format(date, null!));
    }
}