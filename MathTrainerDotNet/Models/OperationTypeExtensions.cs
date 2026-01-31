namespace MathTrainerDotNet.Models;

using Services.Localization;

/// <summary>
/// Provides extension methods for the <see cref="OperationType"/> enumeration, enabling additional functionality such as
/// format conversions and retrieval of associated operations.
/// </summary>
public static class OperationTypeExtensions
{
    /// <summary>
    /// Converts the specified operation type into a string representation suitable for use in file names.
    /// </summary>
    /// <param name="type">The operation type to be converted.</param>
    /// <param name="localizationService">The localization service used to retrieve localized strings.</param>
    /// <returns>A string representation of the operation type, specifically formatted for file naming.</returns>
    public static string ToFileNameString(this OperationType type, ILocalizationService localizationService) => type switch
    {
        OperationType.Multiplication => localizationService["FileNameMultiplication"],
        OperationType.Division => localizationService["FileNameDivision"],
        OperationType.Addition => localizationService["FileNameAddition"],
        OperationType.Subtraction => localizationService["FileNameSubtraction"],
        OperationType.MultiplicationDivision => localizationService["FileNameMultiplicationDivision"],
        OperationType.AdditionSubtraction => localizationService["FileNameAdditionSubtraction"],
        OperationType.All => localizationService["FileNameAllOperation"],
        _ => type.ToString()
    };

    /// <summary>
    /// Retrieves the possible single operations for a given operation type.
    /// </summary>
    /// <param name="type">The operation type for which the possible single operations are determined.</param>
    /// <returns>An array of single operations corresponding to the specified operation type.</returns>
    public static SingleOperation[] GetPossibleOperations(this OperationType type) => type switch
    {
        OperationType.Multiplication => [SingleOperation.Multiplication],
        OperationType.Division => [SingleOperation.Division],
        OperationType.Addition => [SingleOperation.Addition],
        OperationType.Subtraction => [SingleOperation.Subtraction],
        OperationType.MultiplicationDivision => [SingleOperation.Multiplication, SingleOperation.Division],
        OperationType.AdditionSubtraction => [SingleOperation.Addition, SingleOperation.Subtraction],
        OperationType.All => [SingleOperation.Multiplication, SingleOperation.Division, SingleOperation.Addition, SingleOperation.Subtraction],
        _ => [SingleOperation.Addition]
    };
}