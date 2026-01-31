namespace MathTrainerDotNet.Services.Id;

using System.Security.Cryptography;

/// <summary>
/// Provides functionality for generating random alphanumeric identifiers suitable for public use.
/// Avoids ambiguous characters to improve readability.
/// </summary>
public sealed class PublicIdService : IPublicIdService
{
    private const string Chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

    /// <summary>
    /// Generates a random alphanumeric string consisting of 6 characters.
    /// The string is composed of characters from a predefined set excluding ambiguous ones
    /// (e.g., letters like 'I' or 'O', and digits like '0' or '1') for improved readability.
    /// </summary>
    /// <returns>A randomly generated 6-character alphanumeric string.</returns>
    public string GeneratePublicId()
    {
        Span<char> result = stackalloc char[6];

        for (int i = 0; i < 6; i++)
        {
            result[i] = Chars[RandomNumberGenerator.GetInt32(Chars.Length)];
        }

        return new string(result);
    }
}