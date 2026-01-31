namespace MathTrainerDotNet.Models;

/// <summary>
/// Represents the type of mathematical operations available in the system.
/// </summary>
public enum OperationType
{
    /// <summary>
    /// Represents the multiplication operation type in the system.
    /// This operation is used to generate or process exercises involving multiplication.
    /// </summary>
    Multiplication = 0,

    /// <summary>
    /// Represents the division operation type in the system.
    /// This operation is used to generate or process exercises involving division.
    /// </summary>
    Division = 1,

    /// <summary>
    /// Represents the addition operation type in the system.
    /// This operation is used to create or handle exercises involving addition.
    /// </summary>
    Addition = 2,

    /// <summary>
    /// Represents the subtraction operation type in the system.
    /// This operation is used to generate or process exercises involving subtraction.
    /// </summary>
    Subtraction = 3,

    /// <summary>
    /// Represents a combined operation type that includes both multiplication and division.
    /// This operation type is used for exercises that require a mix of multiplication
    /// and division operations, providing a broader scope of mathematical problems.
    /// </summary>
    MultiplicationDivision = 10,

    /// <summary>
    /// Represents a combined operation type in the system that includes both addition and subtraction.
    /// This operation is used to generate or process exercises involving either addition or subtraction.
    /// </summary>
    AdditionSubtraction = 11,

    /// <summary>
    /// Represents all available mathematical operation types in the system.
    /// This option is used to signify that exercises can involve any combination of supported operations.
    /// </summary>
    All = 12
}