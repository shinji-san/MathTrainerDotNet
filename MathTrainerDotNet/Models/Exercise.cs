namespace MathTrainerDotNet.Models;

using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json;

/// <summary>
/// Represents a single mathematical exercise with customizable operands and operators.
/// The class supports serialization and deserialization of operands and operators,
/// and provides methods for performing calculations respecting operator precedence.
/// </summary>
public sealed class Exercise : IEquatable<Exercise>
{
    /// <summary>
    /// Represents the collection of numerical values (operands) used in the mathematical exercise.
    /// These values will be combined with operators to form and calculate expressions.
    /// The operands are serialized to and deserialized from the JSON property <c>OperandsJson</c>,
    /// enabling persistence and retrieval from storage.
    /// </summary>
    private ImmutableArray<int>? operands;

    /// <summary>
    /// Represents the collection of operators used in a mathematical exercise.
    /// These operators define the operations (e.g., addition, subtraction, etc.) to be performed
    /// between the operands in the exercise. The operators are serialized to and deserialized
    /// from the JSON property <c>OperatorsJson</c>, supporting persistence and retrieval from storage.
    /// </summary>
    private ImmutableArray<SingleOperation>? operators;

    /// <summary>
    /// The unique identifier for an exercise. This property serves as the primary key
    /// in the database and is used to uniquely identify each exercise instance.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Represents the foreign key that associates an exercise with its corresponding exercise set.
    /// This property ensures that each exercise belongs to a specific exercise set and enforces the
    /// relationship in the database.
    /// </summary>
    public int ExerciseSetId { get; init; }

    /// <summary>
    /// Represents the operands of a mathematical exercise in a JSON-encoded array format.
    /// This property is used to store the list of operands as a serialized string for persistence.
    /// </summary>
    public string OperandsJson { get; set; } = "[]";

    /// <summary>
    /// Stores the mathematical operators as a JSON-encoded array. This property is used in conjunction with
    /// operands to represent the structure of an exercise. Typical operators include addition, subtraction,
    /// multiplication, and division, and their sequence determines the operation order in calculations.
    /// </summary>
    public string OperatorsJson { get; set; } = "[]";

    /// <summary>
    /// Represents the position of an exercise within an ordered sequence.
    /// This property is used to define the relative order of exercises in a set
    /// and facilitates sorting and retrieval in the desired sequence.
    /// </summary>
    public int OrderIndex { get; set; }

    /// <summary>
    /// Represents the set to which a mathematical exercise belongs. This property establishes
    /// the relationship between an exercise and its corresponding exercise set, enabling
    /// grouping of exercises under a shared context. It is a required, read-only reference
    /// to an <see cref="ExerciseSet"/> instance.
    /// </summary>
    public ExerciseSet ExerciseSet { get; init; } = null!;

    /// <summary>
    /// Gets or sets the collection of operands used in the exercise.
    /// </summary>
    [NotMapped]
    public ImmutableArray<int> Operands
    {
        get
        {
            this.operands ??= JsonSerializer.Deserialize<ImmutableArray<int>>(this.OperandsJson);
            return this.operands ?? [];
        }
        init
        {
            this.operands = value;
            this.OperandsJson = JsonSerializer.Serialize(value);
        }
    }

    /// <summary>
    /// Gets or sets the collection of operators associated with the exercise.
    /// </summary>
    [NotMapped]
    public ImmutableArray<SingleOperation> Operators
    {
        get
        {
            this.operators ??= JsonSerializer.Deserialize<ImmutableArray<SingleOperation>>(this.OperatorsJson);
            return this.operators ?? [];
        }
        init
        {
            this.operators = value;
            this.OperatorsJson = JsonSerializer.Serialize(value);
        }
    }

    /// <summary>
    /// Creates a string representation of the exercise, combining operands and operators
    /// in the form of a mathematical expression.
    /// </summary>
    /// <returns>A formatted string representing the exercise, showing the operands and operators
    /// in sequence, followed by an equals sign. If no operands are present, it returns "? = ".</returns>
    public string ToDisplayString()
    {
        var currentOperands = this.Operands;
        var currentOperators = this.Operators;

        if (currentOperands.Length == 0)
        {
            return "? = ";
        }

        var sb = new StringBuilder();
        sb.Append(currentOperands[0]);
        for (int i = 0; i < currentOperators.Length && i + 1 < currentOperands.Length; i++)
        {
            sb.Append($" {currentOperators[i].GetSymbol()}");
            sb.Append($" {currentOperands[i + 1]}");
        }

        sb.Append(" = ");

        return sb.ToString();
    }

    /// <summary>
    /// Computes the result of the exercise by applying the correct operator precedence
    /// (multiplication and division before addition and subtraction).
    /// </summary>
    /// <returns>The calculated result as an integer after evaluating the operators and operands
    /// in the correct order.</returns>
    public int CalculateResult()
    {
        return CalculateResult(this.Operands, this.Operators);
    }

    /// <summary>
    /// Calculates the result of an exercise using the given operands and operators.
    /// </summary>
    /// <param name="operands">A collection of integers representing the operands for the calculations.</param>
    /// <param name="operators">A collection of single operations specifying the sequence of operations to apply to the operands.</param>
    /// <returns>An integer representing the calculated result of the exercise.</returns>
    public static int CalculateResult(ImmutableArray<int> operands, ImmutableArray<SingleOperation> operators)
    {
        switch (operands.Length)
        {
            case 0:
                return 0;
            case 1:
                return operands[0];
        }

        var intermediateValues = new List<int> { operands[0] };
        var remainingOps = new List<SingleOperation>();

        for (int i = 0; i < operators.Length; i++)
        {
            var singleOperation = operators[i];
            var nextOperand = operands[i + 1];

            switch (singleOperation)
            {
                case SingleOperation.Multiplication:
                    intermediateValues[^1] *= nextOperand;
                    break;
                case SingleOperation.Division when nextOperand == 0:
                    throw new DivideByZeroException();
                case SingleOperation.Division:
                    intermediateValues[^1] /= nextOperand;
                    break;
                default:
                    intermediateValues.Add(nextOperand);
                    remainingOps.Add(singleOperation);
                    break;
            }
        }

        int result = intermediateValues[0];
        for (int i = 0; i < remainingOps.Count; i++)
        {
            result = remainingOps[i] switch
            {
                SingleOperation.Addition => result + intermediateValues[i + 1],
                SingleOperation.Subtraction => result - intermediateValues[i + 1],
                _ => result
            };
        }

        return result;
    }

    /// <summary>
    /// Determines whether the current instance is equal to another instance of the <see cref="Exercise"/> class.
    /// Compares the sequences of operands and operators for equivalence.
    /// </summary>
    /// <param name="other">The <see cref="Exercise"/> instance to compare with the current instance.</param>
    /// <returns>
    /// <see langword="true"/> if the current instance and <paramref name="other"/> are equal;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public bool Equals(Exercise? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return this.Operands.SequenceEqual(other.Operands) && this.Operators.SequenceEqual(other.Operators);
    }

    /// <summary>
    /// Determines whether the current instance is equal to a specified object.
    /// Compares the object to the current instance by checking if it is an <see cref="Exercise"/>
    /// and evaluating equivalence of operands and operators.
    /// </summary>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns>
    /// <see langword="true"/> if the specified object is an instance of <see cref="Exercise"/>
    /// and is equal to the current instance; otherwise, <see langword="false"/>.
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

        return obj.GetType() == this.GetType() && this.Equals((Exercise)obj);
    }

    /// <summary>
    /// Generates a hash code for the current exercise based on its operands and operators.
    /// </summary>
    /// <returns>An integer representing the hash code of the exercise, derived from the operands
    /// and operators.</returns>
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        foreach (var operand in this.Operands)
        {
            hashCode.Add(operand);
        }

        foreach (var singleOperation in this.Operators)
        {
            hashCode.Add(singleOperation);
        }

        return hashCode.ToHashCode();
    }

    /// <summary>
    /// Determines whether two instances of the Exercise class are equal.
    /// </summary>
    /// <param name="left">The first Exercise instance to compare.</param>
    /// <param name="right">The second Exercise instance to compare.</param>
    /// <returns><see langword="true"/> if the two instances are equal; otherwise, <see langword="false"/>.</returns>   
    public static bool operator ==(Exercise? left, Exercise? right) => Equals(left, right);

    /// <summary>
    /// Compares two <see cref="Exercise"/> objects for inequality.
    /// </summary>
    /// <param name="left">The first <see cref="Exercise"/> instance to compare.</param>
    /// <param name="right">The second <see cref="Exercise"/> instance to compare.</param>
    /// <returns><see langword="true"/> if the two instances are not equal; otherwise, <see langword="false"/>.</returns>
    public static bool operator !=(Exercise? left, Exercise? right) => !Equals(left, right);
}