namespace MathTrainerDotNet.ViewModels;

using Models;

public interface IExerciseViewModel
{
    /// <summary>
    /// Retrieves the collection of integers used as input values in the exercise.
    /// These integers serve as the primary components upon which mathematical operations
    /// are performed to compute the exercise's result.
    /// </summary>
    IEnumerable<int> Operands { get; }

    /// <summary>
    /// Represents the collection of mathematical operations utilized in the exercise.
    /// These operations define the sequence of arithmetic actions, such as addition,
    /// subtraction, multiplication, or division, that are applied to the operands
    /// to calculate the result of the exercise.
    /// </summary>
    IEnumerable<SingleOperation> Operators { get; }

    /// <summary>
    /// Represents the answer provided by the user for the exercise.
    /// This value is typically entered as a string and may be processed
    /// or validated to determine its correctness in relation to the exercise's requirements.
    /// </summary>
    string UserAnswer { get; set; }

    /// <summary>
    /// Indicates whether the current exercise's response is correct.
    /// A value of true represents a correct response, false indicates
    /// an incorrect response, and null signifies that no evaluation
    /// has been performed yet.
    /// </summary>
    bool? IsCorrect { get; set; }

    /// <summary>
    /// Specifies whether the result of the exercise should be displayed.
    /// This property is used to control the visibility of the outcome,
    /// allowing users to reveal or hide the solution as needed.
    /// </summary>
    bool ShowResult { get; set; }

    /// <summary>
    /// Represents the calculated result of the exercise, determined by applying the sequence
    /// of operators to the operands with correct operator precedence (e.g., multiplication
    /// and division are performed before addition and subtraction).
    /// </summary>
    int CorrectResult { get; }

    /// <summary>
    /// Represents the textual representation of the exercise, typically combining the operands
    /// and operators to form a human-readable mathematical expression.
    /// </summary>
    string DisplayText { get; }

    /// <summary>
    /// Validates the user's answer for an exercise and performs necessary actions
    /// based on whether the answer is correct. This method is typically called
    /// after the user submits their answer in an exercise.
    /// </summary>
    void CheckAnswer();
}