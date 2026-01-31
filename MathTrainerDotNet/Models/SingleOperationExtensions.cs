namespace MathTrainerDotNet.Models;

/// <summary>
/// Provides extension methods for the <see cref="SingleOperation"/> enumeration.
/// </summary>
public static class SingleOperationExtensions
{
    /// <summary>
    /// Returns the symbolic representation of the given single operation.
    /// </summary>
    /// <param name="operation">The single operation for which the symbol is retrieved.</param>
    /// <returns>The symbol corresponding to the specified operation.</returns>
    public static string GetSymbol(this SingleOperation operation) => operation switch
    {
        SingleOperation.Multiplication => "×",
        SingleOperation.Division => "÷",
        SingleOperation.Addition => "+",
        SingleOperation.Subtraction => "−",
        _ => "?"
    };

}