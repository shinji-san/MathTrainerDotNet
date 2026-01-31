namespace MathTrainerDotNet.Models;

using ViewModels;

/// <summary>
/// Result of loading an exercise set.
/// </summary>
public class LoadExercisesResult
{
    public bool Found { get; init; }
    public List<IExerciseViewModel> Exercises { get; init; } = new();
    public int MinValue { get; init; }
    public int MaxValue { get; init; }
    public OperationType OperationType { get; init; }
    public string StudentName { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public int OperandCount { get; init; } = 2;
}
