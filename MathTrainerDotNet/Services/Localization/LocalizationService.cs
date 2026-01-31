namespace MathTrainerDotNet.Services.Localization;

using System.Globalization;
using System.Resources;

/// <summary>
/// Provides localization functionality for the application.
/// This service utilizes the ResourceManager to load translations from .resx resource files.
/// </summary>
public class LocalizationService : ILocalizationService
{
    /// <summary>
    /// Represents a resource manager used to retrieve localized strings from resource files.
    /// This instance is utilized to manage and access resources for localization purposes
    /// within the application.
    /// </summary>
    private readonly ResourceManager resourceManager;

    /// <summary>
    /// Event triggered whenever the application's language or culture is changed.
    /// Subscribed methods are notified to handle updates related to switching languages.
    /// </summary>
    public event Action? OnLanguageChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalizationService"/> class.
    /// </summary>
    public LocalizationService()
    {
        //// ResourceManage for Strings.resx files
        this.resourceManager =
            new ResourceManager("MathTrainerDotNet.Resources.Strings", typeof(LocalizationService).Assembly);

        //// Default: Deutsch (German)
        this.CurrentCulture = new CultureInfo("de");
    }

    /// <summary>
    /// Indicates the current language of the application in a two-letter ISO 639-1 format.
    /// This property provides a shorthand representation of the current culture's language.
    /// </summary>
    public string CurrentLanguage => this.CurrentCulture.TwoLetterISOLanguageName;

    /// <summary>
    /// Indicates whether the current culture of the application is set to German.
    /// Returns true if the two-letter ISO language name of the current culture is "de",
    /// otherwise false.
    /// </summary>
    public bool IsGerman => this.CurrentCulture.TwoLetterISOLanguageName == "de";

    /// <summary>
    /// Gets the currently active culture used for localization within the application.
    /// This variable determines the language and cultural formatting for retrieving localized
    /// resources and formatting content accordingly.
    /// </summary>
    public CultureInfo CurrentCulture { get; private set; }

    /// <summary>
    /// Retrieves a localized string corresponding to the specified key.
    /// </summary>
    /// <param name="key">The key identifying the desired localized string.</param>
    /// <returns>The localized string associated with the given key.</returns>
    public string this[string key] => this.GetString(key);

    /// <summary>
    /// Retrieves the localized string for the specified key.
    /// </summary>
    /// <param name="key">The key identifying the localized string to retrieve.</param>
    /// <returns>The localized string associated with the specified key. If the key is not found, a placeholder with
    /// the key's name is returned.</returns>
    public string GetString(string key)
    {
        try
        {
            var value = this.resourceManager.GetString(key, this.CurrentCulture);
            return value ?? $"[{key}]";
        }
        catch
        {
            return $"[{key}]";
        }
    }

    /// <summary>
    /// Retrieves the localized string for the specified key and formats it with the provided arguments.
    /// </summary>
    /// <param name="key">The key representing the desired localized string.</param>
    /// <param name="args">An optional array of arguments to format the localized string.</param>
    /// <returns>The formatted localized string if arguments are provided; otherwise, the raw localized string.</returns>
    public string GetString(string key, params object[] args)
    {
        var template = this.GetString(key);
        try
        {
            return string.Format(template, args);
        }
        catch
        {
            return template;
        }
    }

    /// <summary>
    /// Set the current language.
    /// </summary>
    public void SetLanguage(string language)
    {
        var newCulture = new CultureInfo(language);
        if (this.CurrentCulture.TwoLetterISOLanguageName == newCulture.TwoLetterISOLanguageName)
        {
            return;
        }

        this.CurrentCulture = newCulture;
        this.OnLanguageChanged?.Invoke();
    }

    /// <summary>
    /// Toggles the application's language between English and German.
    /// </summary>
    public void ToggleLanguage()
    {
        this.SetLanguage(this.IsGerman ? "en" : "de");
    }
}