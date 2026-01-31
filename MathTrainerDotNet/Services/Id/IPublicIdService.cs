namespace MathTrainerDotNet.Services.Id;

/// <summary>
/// Defines a service for generating random alphanumeric identifiers.
/// These identifiers are designed for public-facing usage and exclude ambiguous characters
/// to enhance readability.
/// </summary>
public interface IPublicIdService
{
    /// <summary>
    /// Generates a random alphanumeric string consisting of 6 characters.
    /// The string is composed of characters from a predefined set excluding ambiguous ones
    /// (e.g., letters like 'I' or 'O', and digits like '0' or '1') for improved readability.
    /// </summary>
    /// <returns>A randomly generated 6-character alphanumeric string.</returns>
    string GeneratePublicId();
}