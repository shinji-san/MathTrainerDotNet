namespace MathTrainerDotNet.ViewModels;

/// <summary>
/// Represents a view model for an exercise set, containing metadata and a collection of exercises.
/// </summary>
public sealed record ExerciseSetViewModel(
    string PublicId,
    IEnumerable<IExerciseViewModel> Exercises,
    DateTime CreatedAt,
    int OperandCount);
