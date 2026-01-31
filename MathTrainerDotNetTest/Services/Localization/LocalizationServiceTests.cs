namespace MathTrainerDotNetTest.Services.Localization;

public class LocalizationServiceTests
{
    [Fact]
    public void Constructor_ShouldDefaultToGerman()
    {
        // Arrange & Act
        var service = new LocalizationService();
        
        // Assert
        Assert.Equal("de", service.CurrentLanguage);
        Assert.True(service.IsGerman);
    }
    
    [Fact]
    public void SetLanguage_ToEnglish_ShouldUpdateCurrentLanguage()
    {
        // Arrange
        var service = new LocalizationService();
        
        // Act
        service.SetLanguage("en");
        
        // Assert
        Assert.Equal("en", service.CurrentLanguage);
        Assert.False(service.IsGerman);
    }
    
    [Fact]
    public void SetLanguage_ToGerman_ShouldUpdateCurrentLanguage()
    {
        // Arrange
        var service = new LocalizationService();
        service.SetLanguage("en");
        
        // Act
        service.SetLanguage("de");
        
        // Assert
        Assert.Equal("de", service.CurrentLanguage);
        Assert.True(service.IsGerman);
    }
    
    [Fact]
    public void SetLanguage_ShouldFireOnLanguageChangedEvent()
    {
        // Arrange
        var service = new LocalizationService();
        var eventFired = false;
        service.OnLanguageChanged += () => eventFired = true;
        
        // Act
        service.SetLanguage("en");
        
        // Assert
        Assert.True(eventFired);
    }
    
    [Fact]
    public void SetLanguage_SameLanguage_ShouldNotFireEvent()
    {
        // Arrange
        var service = new LocalizationService();
        var eventFired = false;
        service.OnLanguageChanged += () => eventFired = true;
        
        // Act
        service.SetLanguage("de"); // Already German
        
        // Assert
        Assert.False(eventFired);
    }
    
    [Fact]
    public void ToggleLanguage_FromGerman_ShouldSwitchToEnglish()
    {
        // Arrange
        var service = new LocalizationService();
        Assert.True(service.IsGerman);
        
        // Act
        service.ToggleLanguage();
        
        // Assert
        Assert.False(service.IsGerman);
        Assert.Equal("en", service.CurrentLanguage);
    }
    
    [Fact]
    public void ToggleLanguage_FromEnglish_ShouldSwitchToGerman()
    {
        // Arrange
        var service = new LocalizationService();
        service.SetLanguage("en");
        
        // Act
        service.ToggleLanguage();
        
        // Assert
        Assert.True(service.IsGerman);
        Assert.Equal("de", service.CurrentLanguage);
    }
    
    [Fact]
    public void Indexer_ValidKey_ShouldReturnValue()
    {
        // Arrange
        var service = new LocalizationService();
        
        // Act
        var result = service["AppTitle"];
        
        // Assert
        Assert.NotNull(result);
        Assert.NotEqual("[AppTitle]", result);
    }
    
    [Fact]
    public void Indexer_InvalidKey_ShouldReturnBracketedKey()
    {
        // Arrange
        var service = new LocalizationService();
        
        // Act
        var result = service["NonExistentKey"];
        
        // Assert
        Assert.Equal("[NonExistentKey]", result);
    }
    
    [Fact]
    public void GetString_German_ShouldReturnGermanText()
    {
        // Arrange
        var service = new LocalizationService();
        service.SetLanguage("de");
        
        // Act
        var result = service.GetString("AppTitle");
        
        // Assert
        Assert.Equal("Rechentrainer", result);
    }
    
    [Fact]
    public void GetString_English_ShouldReturnEnglishText()
    {
        // Arrange
        var service = new LocalizationService();
        service.SetLanguage("en");
        
        // Act
        var result = service.GetString("AppTitle");
        
        // Assert
        Assert.Equal("Math Trainer", result);
    }
    
    [Fact]
    public void GetString_WithFormatArgs_ShouldFormatCorrectly()
    {
        // Arrange
        var service = new LocalizationService();
        
        // Act
        var result = service.GetString("ErrorNotFound", "ABC123");
        
        // Assert
        Assert.Contains("ABC123", result);
    }
    
    [Fact]
    public void CurrentCulture_ShouldReturnCorrectCultureInfo()
    {
        // Arrange
        var service = new LocalizationService();
        
        // Act
        var culture = service.CurrentCulture;
        
        // Assert
        Assert.Equal("de", culture.TwoLetterISOLanguageName);
    }
}
