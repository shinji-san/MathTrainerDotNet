namespace MathTrainerDotNet.Models;

/// <summary>
/// Represents a set of exercises created for a specific student.
/// This class contains metadata and configuration details for generating and managing exercises,
/// including the number of exercises, their range of values, the type of mathematical operation,
/// and associations with a specific student.
/// </summary>
/// <remarks>
/// Instances of this class are immutable after initialization. This ensures the integrity
/// of the exercise set's configuration once it has been created. The class also includes
/// equality comparison methods to determine equivalence between two exercise sets based on their properties.
/// </remarks>
public sealed class ExerciseSet : IEquatable<ExerciseSet>
{
    /// <summary>
    /// A unique identifier for the exercise set, primarily used for internal database relations and integrity constraints.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// A unique, user-friendly identifier for the exercise set, used for lookup and retrieval purposes. (e.g. "ABC123").
    /// </summary>
    public string PublicId { get; init; } = string.Empty;

    /// <summary>
    /// The unique identifier of the student associated with the exercise set.
    /// </summary>
    public int StudentId { get; init; }

    /// <summary>
    /// Specifies the minimum value allowed for the operands in the exercises within the exercise set.
    /// </summary>
    public int MinValue { get; init; }

    /// <summary>
    /// Specifies the maximum numeric value that can be used in the exercises within the set.
    /// This defines the upper boundary for the range of numbers utilized in exercise generation.
    /// </summary>
    public int MaxValue { get; init; }

    /// <summary>
    /// Specifies the total number of exercises included in an exercise set.
    /// This property determines how many exercises are generated
    /// and stored for a specific series within the system.
    /// </summary>
    public int ExerciseCount { get; init; }

    /// <summary>
    /// Specifies the number of operands used in the exercises within the exercise set.
    /// Determines how many numbers are involved in each mathematical operation.
    /// (minimum is 2).
    /// </summary>
    public int OperandCount { get; init; } = 2;

    /// <summary>
    /// Specifies the type of mathematical operations to be utilized within the exercise set,
    /// such as addition, subtraction, multiplication, division, or a combination thereof.
    /// </summary>
    public OperationType OperationType { get; init; }

    /// <summary>
    /// The date and time at which the exercise set was created, represented in Coordinated Universal Time (UTC).
    /// Used to track the creation timestamp of the exercise set.
    /// </summary>
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Represents the student associated with the exercise set. This property establishes a relationship between an exercise set and the specific
    /// student it belongs to, enabling access to student details such as their name and associated exercise history.
    /// </summary>
    public Student Student { get; init; } = null!;

    /// <summary>
    /// A collection of exercises associated with a specific exercise set, where each exercise represents an individual problem or task.
    /// </summary>
    public IEnumerable<Exercise> Exercises { get; init; } = new List<Exercise>();

    /// <summary>
    /// Determines whether the current <see cref="ExerciseSet"/> instance is equal to another.
    /// Comparison is based on the values of all properties.
    /// </summary>
    /// <param name="other">The <see cref="ExerciseSet"/> instance to compare with the current instance.</param>
    /// <returns><see langword="true"/> if the specified <see cref="ExerciseSet"/> instance is equal to the current
    /// instance; otherwise, <see langword="false"/>.</returns>
    public bool Equals(ExerciseSet? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return this.Id == other.Id && this.PublicId == other.PublicId && this.StudentId == other.StudentId &&
               this.MinValue == other.MinValue && this.MaxValue == other.MaxValue &&
               this.ExerciseCount == other.ExerciseCount && this.OperandCount == other.OperandCount &&
               this.OperationType == other.OperationType && this.CreatedAt.Equals(other.CreatedAt) &&
               this.Student.Equals(other.Student) && this.Exercises.SequenceEqual(other.Exercises);
    }

    /// <summary>
    /// Determines whether the current <see cref="ExerciseSet"/> instance is equal to a specified object.
    /// </summary>
    /// <param name="obj">The object to compare with the current <see cref="ExerciseSet"/> instance.</param>
    /// <returns><see langword="true"/> if the specified object is an <see cref="ExerciseSet"/> and is equal to the
    /// current instance; otherwise, <see langword="false"/>.</returns>
    public override bool Equals(object? obj) =>
        ReferenceEquals(this, obj) || obj is ExerciseSet other && this.Equals(other);

    /// <summary>
    /// Serves as the default hash function for the <see cref="ExerciseSet"/> class.
    /// Generates a hash code based on the values of all properties in the instance.
    /// </summary>
    /// <returns>
    /// An <see cref="int"/> representing the hash code of the current <see cref="ExerciseSet"/> instance.
    /// </returns>
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(this.Id);
        hashCode.Add(this.PublicId);
        hashCode.Add(this.StudentId);
        hashCode.Add(this.MinValue);
        hashCode.Add(this.MaxValue);
        hashCode.Add(this.ExerciseCount);
        hashCode.Add(this.OperandCount);
        hashCode.Add((int)this.OperationType);
        hashCode.Add(this.CreatedAt);
        hashCode.Add(this.Student);
        foreach (var exercise in this.Exercises)
        {
            hashCode.Add(exercise);
        }

        return hashCode.ToHashCode();
    }

    /// <summary>
    /// Determines whether two <see cref="ExerciseSet"/> instances are equal.
    /// </summary>
    /// <param name="left">The first <see cref="ExerciseSet"/> instance to compare.</param>
    /// <param name="right">The second <see cref="ExerciseSet"/> instance to compare.</param>
    /// <returns>
    /// <see langword="true"/> if the two <see cref="ExerciseSet"/> instances are equal; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator ==(ExerciseSet? left, ExerciseSet? right) => Equals(left, right);

    /// <summary>
    /// Determines whether two <see cref="ExerciseSet"/> instances are not equal by comparing their property values.
    /// </summary>
    /// <param name="left">The first <see cref="ExerciseSet"/> instance to compare.</param>
    /// <param name="right">The second <see cref="ExerciseSet"/> instance to compare.</param>
    /// <returns>
    /// <see langword="true"/> if the two <see cref="ExerciseSet"/> instances are not equal; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator !=(ExerciseSet? left, ExerciseSet? right) => !Equals(left, right);
}