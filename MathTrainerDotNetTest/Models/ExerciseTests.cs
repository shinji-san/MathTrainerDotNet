namespace MathTrainerDotNetTest.Models;

public class ExerciseTests
{
    // [Fact]
    // public void SetOperands_ShouldSerializeCorrectly()
    // {
    //     // Arrange
    //     var exercise = new Exercise();
    //     var operands = new List<int> { 5, 3, 2 };
    //     
    //     // Act
    //     exercise.SetOperands(operands);
    //     var result = exercise.GetOperands();
    //     
    //     // Assert
    //     Assert.Equal(operands, result);
    // }
    //
    // [Fact]
    // public void SetOperators_ShouldSerializeCorrectly()
    // {
    //     // Arrange
    //     var exercise = new Exercise();
    //     var operators = new List<SingleOperation> 
    //     { 
    //         SingleOperation.Addition, 
    //         SingleOperation.Multiplication 
    //     };
    //     
    //     // Act
    //     exercise.SetOperators(operators);
    //     var result = exercise.GetOperators();
    //     
    //     // Assert
    //     Assert.Equal(operators, result);
    // }

    [Theory]
    [InlineData(new[] { 5, 3 }, new[] { SingleOperation.Addition }, 8)]
    [InlineData(new[] { 10, 4 }, new[] { SingleOperation.Subtraction }, 6)]
    [InlineData(new[] { 6, 7 }, new[] { SingleOperation.Multiplication }, 42)]
    [InlineData(new[] { 20, 4 }, new[] { SingleOperation.Division }, 5)]
    public void CalculateResult_TwoOperands_ShouldReturnCorrectResult(
        int[] operands,
        SingleOperation[] operators,
        int expected)
    {
        // Arrange
        var exercise = new Exercise
        {
            Operands = [..operands],
            Operators = [..operators]
        };

        // Act
        var result = exercise.CalculateResult();

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void CalculateResult_ThreeOperands_ShouldCalculateLeftToRight()
    {
        // Arrange: 10 + 5 - 3 = 12 (left to right, only addition/subtraction)
        var exercise = new Exercise
        {
            Operands = [10, 5, 3],
            Operators = [SingleOperation.Addition, SingleOperation.Subtraction]
        };

        // Act
        var result = exercise.CalculateResult();

        // Assert
        Assert.Equal(12, result);
    }

    [Fact]
    public void CalculateResult_MixedOperations_ShouldRespectPrecedence()
    {
        // Arrange: 2 + 3 × 4 = 14 (Punktrechnung vor Strichrechnung: 3 × 4 = 12, dann 2 + 12 = 14)
        var exercise = new Exercise
        {
            Operands = [2, 3, 4],
            Operators = [SingleOperation.Addition, SingleOperation.Multiplication]
        };

        // Act
        var result = exercise.CalculateResult();

        // Assert
        Assert.Equal(14, result); // 2 + (3 × 4) = 14
    }

    [Fact]
    public void CalculateResult_ComplexMixedOperations_ShouldRespectPrecedence()
    {
        // Arrange: 10 - 2 × 3 + 4 = 10 - 6 + 4 = 8
        var exercise = new Exercise
        {
            Operands = [10, 2, 3, 4],
            Operators =
            [
                SingleOperation.Subtraction,
                SingleOperation.Multiplication,
                SingleOperation.Addition
            ]
        };

        // Act
        var result = exercise.CalculateResult();

        // Assert
        Assert.Equal(8, result); // 10 - (2 × 3) + 4 = 10 - 6 + 4 = 8
    }

    [Fact]
    public void CalculateResult_DivisionBeforeAddition_ShouldRespectPrecedence()
    {
        // Arrange: 5 + 12 ÷ 4 = 5 + 3 = 8
        var exercise = new Exercise
        {
            Operands = [5, 12, 4],
            Operators = [SingleOperation.Addition, SingleOperation.Division]
        };

        // Act
        var result = exercise.CalculateResult();

        // Assert
        Assert.Equal(8, result); // 5 + (12 ÷ 4) = 5 + 3 = 8
    }

    [Fact]
    public void CalculateResult_MultipleMultiplications_ShouldCalculateLeftToRight()
    {
        // Arrange: 2 × 3 × 4 = 24 (left to right for same precedence)
        var exercise = new Exercise
        {
            Operands = [2, 3, 4],
            Operators = [SingleOperation.Multiplication, SingleOperation.Multiplication]
        };

        // Act
        var result = exercise.CalculateResult();

        // Assert
        Assert.Equal(24, result);
    }

    [Fact]
    public void CalculateResult_EmptyOperands_ShouldReturnZero()
    {
        // Arrange
        var exercise = new Exercise
        {
            Operands = [],
            Operators = []
        };

        // Act
        var result = exercise.CalculateResult();

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public void CalculateResult_SingleOperand_ShouldReturnThatOperand()
    {
        // Arrange
        var exercise = new Exercise
        {
            Operands = [42],
            Operators = []
        };

        // Act
        var result = exercise.CalculateResult();

        // Assert
        Assert.Equal(42, result);
    }

    [Theory]
    [InlineData(new[] { 5, 3 }, new[] { SingleOperation.Addition }, "5 + 3 = ")]
    [InlineData(new[] { 10, 4 }, new[] { SingleOperation.Subtraction }, "10 − 4 = ")]
    [InlineData(new[] { 6, 7 }, new[] { SingleOperation.Multiplication }, "6 × 7 = ")]
    [InlineData(new[] { 20, 4 }, new[] { SingleOperation.Division }, "20 ÷ 4 = ")]
    public void ToDisplayString_ShouldFormatCorrectly(
        int[] operands,
        SingleOperation[] operators,
        string expected)
    {
        // Arrange
        var exercise = new Exercise
        {
            Operands = [..operands],
            Operators = [..operators]
        };


        // Act
        var result = exercise.ToDisplayString();

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ToDisplayString_ThreeOperands_ShouldFormatCorrectly()
    {
        // Arrange
        var exercise = new Exercise
        {
            Operands = [10, 5, 3],
            Operators = [SingleOperation.Addition, SingleOperation.Subtraction]
        };

        // Act
        var result = exercise.ToDisplayString();

        // Assert
        Assert.Equal("10 + 5 − 3 = ", result);
    }
}