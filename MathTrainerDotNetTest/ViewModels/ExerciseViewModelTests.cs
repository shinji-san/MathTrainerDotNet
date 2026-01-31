namespace MathTrainerDotNetTest.ViewModels;

public class ExerciseViewModelTests
{
    [Fact]
    public void CorrectResult_Addition_ShouldCalculateCorrectly()
    {
        // Arrange
        var exercise = new Exercise
        {
            Operands = [5, 3],
            Operators = [SingleOperation.Addition]
        };

        var viewModel = new ExerciseViewModel(exercise);
        
        // Act & Assert
        Assert.Equal(8, viewModel.CorrectResult);
    }
    
    [Fact]
    public void CorrectResult_MultipleOperands_ShouldRespectPrecedence()
    {
        // Arrange: 10 + 2 × 5 = 10 + 10 = 20 (Start with Multiplication/Division, then Addition/Subtraction)
        var exercise = new Exercise
        {
            Operands = [10, 2, 5],
            Operators = [SingleOperation.Addition, SingleOperation.Multiplication]
        };

        var viewModel = new ExerciseViewModel(exercise);
        
        // Act & Assert
        Assert.Equal(20, viewModel.CorrectResult); // 10 + (2 × 5) = 20
    }
    
    [Fact]
    public void CorrectResult_OnlyMultiplication_ShouldCalculateLeftToRight()
    {
        // Arrange: 10 × 2 + 5 with only multiplication operators
        var exercise = new Exercise
        {
            Operands = [10, 2, 5],
            Operators = [SingleOperation.Multiplication, SingleOperation.Addition]
        };

        var viewModel = new ExerciseViewModel(exercise);
        
        // Act & Assert
        Assert.Equal(25, viewModel.CorrectResult); // (10 × 2) + 5 = 25
    }
    
    [Fact]
    public void DisplayText_ShouldFormatCorrectly()
    {
        // Arrange
        var exercise = new Exercise
        {
            Operands = [12, 4],
            Operators = [SingleOperation.Division]
        };

        var viewModel = new ExerciseViewModel(exercise);
        
        // Act & Assert
        Assert.Equal("12 ÷ 4 = ", viewModel.DisplayText);
    }
    
    [Theory]
    [InlineData("8", true)]
    [InlineData("7", false)]
    [InlineData("abc", null)]
    [InlineData("", null)]
    [InlineData("  ", null)]
    public void CheckAnswer_ShouldSetIsCorrectAppropriately(string userAnswer, bool? expected)
    {
        // Arrange
        var exercise = new Exercise
        {
            Operands = [5, 3],
            Operators = [SingleOperation.Addition]
        };

        var viewModel = new ExerciseViewModel(exercise)
        {
            UserAnswer = userAnswer
        };

        // Act
        viewModel.CheckAnswer();
        
        // Assert
        Assert.Equal(expected, viewModel.IsCorrect);
    }
    
    [Fact]
    public void CheckAnswer_WithWhitespace_ShouldTrimAndCheck()
    {
        // Arrange
        var exercise = new Exercise
        {
            Operands = [5, 3],
            Operators = [SingleOperation.Addition]
        };

        var viewModel = new ExerciseViewModel(exercise)
        {
            UserAnswer = "  8  "
        };
        
        // Act
        viewModel.CheckAnswer();
        
        // Assert
        Assert.True(viewModel.IsCorrect);
    }
    
    [Fact]
    public void FromExercise_ShouldCreateViewModelCorrectly()
    {
        // Arrange
        var exercise = new Exercise
        {
            Operands = [7, 3, 2],
            Operators = [SingleOperation.Addition, SingleOperation.Subtraction]
        };

        // Act
        var viewModel = ExerciseViewModel.Of(exercise);
        
        // Assert
        Assert.Equal([7, 3, 2], viewModel.Operands);
        Assert.Equal([SingleOperation.Addition, SingleOperation.Subtraction], viewModel.Operators);
        Assert.Equal(8, viewModel.CorrectResult); // 7 + 3 - 2 = 8
    }

    [Fact]
    public void ShowResult_DefaultShouldBeFalse()
    {
        // Arrange & Act
        var viewModel = new ExerciseViewModel();
        
        // Assert
        Assert.False(viewModel.ShowResult);
    }
    
    [Fact]
    public void IsCorrect_DefaultShouldBeNull()
    {
        // Arrange & Act
        var viewModel = new ExerciseViewModel();
        
        // Assert
        Assert.Null(viewModel.IsCorrect);
    }
}
