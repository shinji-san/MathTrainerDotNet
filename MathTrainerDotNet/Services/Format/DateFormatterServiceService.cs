namespace MathTrainerDotNet.Services.Format;

using System.Globalization;

/// <summary>
/// Provides functionality for formatting dates into human-readable strings based on a specified culture.
/// </summary>
public sealed class DateFormatterServiceService : IDateFormatterService
{
    /// <summary>
    /// Formats a specific date into a human-readable string using the user's culture.
    /// </summary>
    /// <param name="date">The date to format.</param>
    /// <param name="userCulture">The user's culture.</param>
    /// <returns>A culture-specific, human-readable date string.</returns>
    public string Format(DateTime date, CultureInfo userCulture)
    {
        ArgumentNullException.ThrowIfNull(userCulture);
        return date.ToString("D", userCulture);
    }
}