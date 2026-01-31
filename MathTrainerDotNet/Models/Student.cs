namespace MathTrainerDotNet.Models;

/// <summary>
/// Represents a student entity with properties for identification, name, creation timestamp, and associated exercise sets.
/// </summary>
public sealed class Student : IEquatable<Student>
{
    /// <summary>
    /// Gets or sets the unique identifier for the student entity.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Gets or sets the name of the student.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the timestamp indicating when the student entity was created.
    /// </summary>
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the collection of exercise sets associated with the student.
    /// </summary>
    public IEnumerable<ExerciseSet> ExerciseSets { get; init; } = new List<ExerciseSet>();

    /// <summary>
    /// Determines whether the current <see cref="Student"/> object is equal to another <see cref="Student"/> object.
    /// </summary>
    /// <param name="other">The <see cref="Student"/> object to compare with the current object.</param>
    /// <returns>
    /// <see langword="true"/> if the specified <see cref="Student"/> object is equal to the current object;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public bool Equals(Student? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return this.Id == other.Id
               && this.Name == other.Name
               && this.CreatedAt.Equals(other.CreatedAt)
               && this.ExerciseSets.SequenceEqual(other.ExerciseSets);
    }

    /// <summary>
    /// Determines whether the current <see cref="Student"/> object is equal to the specified object.
    /// </summary>
    /// <param name="obj">The object to compare with the current <see cref="Student"/> object.</param>
    /// <returns>
    /// <see langword="true"/> if the specified object is a <see cref="Student"/> and is equal to the current object;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != this.GetType())
        {
            return false;
        }

        return this.Equals((Student)obj);
    }

    /// <summary>
    /// Returns a hash code for the current <see cref="Student"/> object.
    /// </summary>
    /// <returns>
    /// A hash code for the current <see cref="Student"/> object, suitable for use in hashing algorithms and data structures
    /// like a hash table.
    /// </returns>
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(this.Id);
        hashCode.Add(this.Name);
        hashCode.Add(this.CreatedAt);
        foreach (var exerciseSet in this.ExerciseSets)
        {
            hashCode.Add(exerciseSet);
        }

        return hashCode.ToHashCode();
    }

    /// <summary>
    /// Determines whether two <see cref="Student"/> objects are equal.
    /// </summary>
    /// <param name="left">The first <see cref="Student"/> object to compare.</param>
    /// <param name="right">The second <see cref="Student"/> object to compare.</param>
    /// <returns>
    /// <see langword="true"/> if the specified <see cref="Student"/> objects are equal;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator ==(Student? left, Student? right) => Equals(left, right);

    /// <summary>
    /// Determines whether two <see cref="Student"/> objects are not equal.
    /// </summary>
    /// <param name="left">The first <see cref="Student"/> object to compare.</param>
    /// <param name="right">The second <see cref="Student"/> object to compare.</param>
    /// <returns>
    /// <see langword="true"/> if the specified <see cref="Student"/> objects are not equal;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator !=(Student? left, Student? right) => !Equals(left, right);
}
