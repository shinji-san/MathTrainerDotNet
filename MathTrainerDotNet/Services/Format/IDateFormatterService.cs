namespace MathTrainerDotNet.Services.Format;

using System.Globalization;

/// <summary>
/// Provides a method to format dates into strings based on a specified culture.
/// </summary>
public interface IDateFormatterService
{
    /// <summary>
    /// Formats a specific date into a human-readable string using the user's culture.
    /// </summary>
    /// <param name="date">The date to format.</param>
    /// <param name="userCulture">The user's culture.</param>
    /// <returns>A culture-specific, human-readable date string.</returns>
    string Format(DateTime date, CultureInfo userCulture);
}