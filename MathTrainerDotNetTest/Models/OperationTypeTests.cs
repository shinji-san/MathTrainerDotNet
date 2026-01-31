namespace MathTrainerDotNetTest.Models;

using Moq;

public class OperationTypeTests
{
    [Theory]
    [InlineData(SingleOperation.Addition, "+")]
    [InlineData(SingleOperation.Subtraction, "−")]
    [InlineData(SingleOperation.Multiplication, "×")]
    [InlineData(SingleOperation.Division, "÷")]
    public void GetSymbol_ShouldReturnCorrectSymbol(SingleOperation operation, string expected)
    {
        // Act
        var result = operation.GetSymbol();
        
        // Assert
        Assert.Equal(expected, result);
    }
    
    [Theory]
    [InlineData(OperationType.Multiplication, "[FileNameMultiplication]")]
    [InlineData(OperationType.Division, "[FileNameDivision]")]
    [InlineData(OperationType.Addition, "[FileNameAddition]")]
    [InlineData(OperationType.Subtraction, "[FileNameSubtraction]")]
    [InlineData(OperationType.MultiplicationDivision, "[FileNameMultiplicationDivision]")]
    [InlineData(OperationType.AdditionSubtraction, "[FileNameAdditionSubtraction]")]
    [InlineData(OperationType.All, "[FileNameAllOperation]")]
    public void ToFileNameString_ShouldReturnCorrectString(OperationType type, string expected)
    {
        // Act
        var localizationServiceMock = new Mock<ILocalizationService>();

        // Setup default localization strings to avoid ArgumentNullException in string.Format
        localizationServiceMock
            .Setup(service => service[It.IsAny<string>()]).Returns((string key) => $"[{key}]");
        localizationServiceMock
            .Setup(service => service.GetString(It.IsAny<string>())).Returns((string key) => $"[{key}]");
        localizationServiceMock
            .Setup(service => service.GetString(It.IsAny<string>(), It.IsAny<object[]>()))
            .Returns((string key, object[] args) => string.Format($"[{key}]", args));
        localizationServiceMock
            .Setup(s => s.CurrentCulture).Returns(new System.Globalization.CultureInfo("de-DE"));
        var result = type.ToFileNameString(localizationServiceMock.Object);
        
        // Assert
        Assert.Equal(expected, result);
    }
    
    [Fact]
    public void GetPossibleOperations_Multiplication_ShouldReturnOnlyMultiplication()
    {
        // Act
        var result = OperationType.Multiplication.GetPossibleOperations();
        
        // Assert
        Assert.Single(result);
        Assert.Contains(SingleOperation.Multiplication, result);
    }
    
    [Fact]
    public void GetPossibleOperations_Division_ShouldReturnOnlyDivision()
    {
        // Act
        var result = OperationType.Division.GetPossibleOperations();
        
        // Assert
        Assert.Single(result);
        Assert.Contains(SingleOperation.Division, result);
    }
    
    [Fact]
    public void GetPossibleOperations_Addition_ShouldReturnOnlyAddition()
    {
        // Act
        var result = OperationType.Addition.GetPossibleOperations();
        
        // Assert
        Assert.Single(result);
        Assert.Contains(SingleOperation.Addition, result);
    }
    
    [Fact]
    public void GetPossibleOperations_Subtraction_ShouldReturnOnlySubtraction()
    {
        // Act
        var result = OperationType.Subtraction.GetPossibleOperations();
        
        // Assert
        Assert.Single(result);
        Assert.Contains(SingleOperation.Subtraction, result);
    }
    
    [Fact]
    public void GetPossibleOperations_MultiplicationDivision_ShouldReturnBoth()
    {
        // Act
        var result = OperationType.MultiplicationDivision.GetPossibleOperations();
        
        // Assert
        Assert.Equal(2, result.Length);
        Assert.Contains(SingleOperation.Multiplication, result);
        Assert.Contains(SingleOperation.Division, result);
    }
    
    [Fact]
    public void GetPossibleOperations_AdditionSubtraction_ShouldReturnBoth()
    {
        // Act
        var result = OperationType.AdditionSubtraction.GetPossibleOperations();
        
        // Assert
        Assert.Equal(2, result.Length);
        Assert.Contains(SingleOperation.Addition, result);
        Assert.Contains(SingleOperation.Subtraction, result);
    }
    
    [Fact]
    public void GetPossibleOperations_All_ShouldReturnAllFour()
    {
        // Act
        var result = OperationType.All.GetPossibleOperations();
        
        // Assert
        Assert.Equal(4, result.Length);
        Assert.Contains(SingleOperation.Addition, result);
        Assert.Contains(SingleOperation.Subtraction, result);
        Assert.Contains(SingleOperation.Multiplication, result);
        Assert.Contains(SingleOperation.Division, result);
    }
}
