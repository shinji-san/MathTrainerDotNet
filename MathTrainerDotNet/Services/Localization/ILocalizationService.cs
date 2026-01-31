namespace MathTrainerDotNet.Services.Localization;

using System.Globalization;

/// <summary>
/// Provides localization services to support multi-language functionality within the application.
/// </summary>
public interface ILocalizationService
{
    /// <summary>
    /// Event triggered whenever the application's language or culture is changed.
    /// Subscribed methods are notified to handle updates related to switching languages.
    /// </summary>
    event Action? OnLanguageChanged;

    /// <summary>
    /// Indicates the current language of the application in a two-letter ISO 639-1 format.
    /// This property provides a shorthand representation of the current culture's language.
    /// </summary>
    string CurrentLanguage { get; }

    /// <summary>
    /// Indicates whether the current culture of the application is set to German.
    /// Returns true if the two-letter ISO language name of the current culture is "de",
    /// otherwise false.
    /// </summary>
    bool IsGerman { get; }

    /// <summary>
    /// Gets the currently active culture used for localization within the application.
    /// This variable determines the language and cultural formatting for retrieving localized
    /// resources and formatting content accordingly.
    /// </summary>
    CultureInfo CurrentCulture { get; }

    /// <summary>
    /// Retrieves a localized string corresponding to the specified key.
    /// </summary>
    /// <param name="key">The key identifying the desired localized string.</param>
    /// <returns>The localized string associated with the given key.</returns>
    string this[string key] { get; }

    /// <summary>
    /// Retrieves the localized string for the specified key.
    /// </summary>
    /// <param name="key">The key identifying the localized string to retrieve.</param>
    /// <returns>The localized string associated with the specified key. If the key is not found, a placeholder with
    /// the key's name is returned.</returns>
    string GetString(string key);

    /// <summary>
    /// Retrieves the localized string for the specified key and formats it with the provided arguments.
    /// </summary>
    /// <param name="key">The key representing the desired localized string.</param>
    /// <param name="args">An optional array of arguments to format the localized string.</param>
    /// <returns>The formatted localized string if arguments are provided; otherwise, the raw localized string.</returns>
    string GetString(string key, params object[] args);

    /// <summary>
    /// Set the current language.
    /// </summary>
    void SetLanguage(string language);

    /// <summary>
    /// Toggles the application's language between English and German.
    /// </summary>
    void ToggleLanguage();
}