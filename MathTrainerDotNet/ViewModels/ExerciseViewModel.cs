namespace MathTrainerDotNet.ViewModels;

using Models;

/// <summary>
/// Represents the view model for displaying an exercise in the user interface.
/// Provides a bridge between the application logic and the UI, exposing relevant exercise data
/// and allowing interaction such as validating the user's answer.
/// </summary>
public class ExerciseViewModel : IExerciseViewModel
{
    /// <summary>
    /// Represents the core exercise data being used or manipulated by the application.
    /// Exposes information about operands, operators, and logic needed for calculations,
    /// and is closely tied to the ExerciseViewModel for displaying operations in the UI.
    /// </summary>
    private readonly Exercise exercise;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExerciseViewModel"/> class.
    /// </summary>
    public ExerciseViewModel()
    {
        this.exercise = new Exercise();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExerciseViewModel"/> class.
    /// </summary>
    /// <param name="exercise">The exercise entity to be wrapped by this view model.</param>
    public ExerciseViewModel(Exercise exercise)
    {
        this.exercise = exercise;
    }

    /// <inheritdoc/>
    public IEnumerable<int> Operands => this.exercise.Operands;

    /// <inheritdoc/>
    public IEnumerable<SingleOperation> Operators => this.exercise.Operators;

    /// <inheritdoc/>
    public string UserAnswer { get; set; } = string.Empty;

    /// <inheritdoc/>
    public bool? IsCorrect { get; set; }

    /// <inheritdoc/>
    public bool ShowResult { get; set; }

    /// <inheritdoc/>
    public int CorrectResult
    {
        get
        {
            this.correctResult ??= this.exercise.CalculateResult();
            return this.correctResult.Value;
        }
    }

    private int? correctResult;

    /// <inheritdoc/>
    public string DisplayText => this.exercise.ToDisplayString();

    /// <inheritdoc/>
    public void CheckAnswer()
    {
        if (int.TryParse(this.UserAnswer.Trim(), out int answer))
        {
            this.IsCorrect = answer == this.CorrectResult;
        }
        else
        {
            this.IsCorrect = null;
        }
    }

    /// <summary>
    /// Creates a new instance of an <see cref="IExerciseViewModel"/> from the provided <see cref="Exercise"/>.
    /// </summary>
    /// <param name="exercise">The exercise to convert into an <see cref="IExerciseViewModel"/>.</param>
    /// <returns>An <see cref="IExerciseViewModel"/> instance initialized with the given <see cref="Exercise"/>.</returns>
    public static IExerciseViewModel Of(Exercise exercise)
    {
        return new ExerciseViewModel(exercise);
    }
}