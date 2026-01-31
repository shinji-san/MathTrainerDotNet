namespace MathTrainerDotNet.Services;

using Id;
using Models;
using Repository;
using System.Security.Cryptography;
using ViewModels;

/// <summary>
/// Provides services for managing exercises, including generating new exercise sets and retrieving existing ones.
/// </summary>
public sealed class ExerciseGeneratorService
{
    /// <summary>
    /// Represents a service for generating random alphanumeric identifiers,
    /// used to create unique public-facing IDs for entities such as exercise sets.
    /// Facilitates the creation of identifiers that are user-friendly and exclude
    /// ambiguous characters for better readability.
    /// </summary>
    private readonly IPublicIdService publicIdService;

    /// <summary>
    /// Represents a repository responsible for handling interactions with the underlying data store
    /// for exercise sets and exercises. Facilitates operations such as creating, retrieving,
    /// and managing exercise-related data within the application.
    /// </summary>
    private readonly IExerciseRepository exerciseRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExerciseGeneratorService"/> class. 
    /// </summary>
    /// <param name="exerciseRepository">The repository responsible for managing exercise data.</param>
    /// <param name="publicIdService">The service responsible for generating public identifiers.</param>
    public ExerciseGeneratorService(IExerciseRepository exerciseRepository, IPublicIdService publicIdService)
    {
        this.publicIdService = publicIdService;
        this.exerciseRepository = exerciseRepository;
    }

    /// <summary>
    /// Asynchronously generates a new set of math exercises for a specific student,
    /// saves them to the database, and returns a view model representing the exercise set.
    /// </summary>
    /// <param name="studentId">The unique identifier of the student for whom the exercises will be generated.</param>
    /// <param name="minValue">Specifies the minimum numeric value allowed for the operands in the exercises within the exercise set.</param>
    /// <param name="maxValue">Specifies the maximum numeric value that can be used in the exercises within the exercise set.</param>
    /// <param name="count">The number of exercises to generate in the set.</param>
    /// <param name="operationType">The type of mathematical operation(s) to include in the exercises.</param>
    /// <param name="operandCount">The number of operands in each generated exercise. Defaults to 2.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains an <see cref="ExerciseSetViewModel"/> instance
    /// representing the generated exercise set.
    /// </returns>
    public async Task<ExerciseSetViewModel> GenerateExercisesAsync(
        int studentId,
        int minValue,
        int maxValue,
        int count,
        OperationType operationType,
        int operandCount = 2)
    {
        //// Minimum 2 operands
        operandCount = Math.Max(2, operandCount);
        
        var possibleOperations = operationType.GetPossibleOperations();
        var exercises = GenerateExercisesList(minValue, maxValue, count, operandCount, possibleOperations);
        
        var publicId = this.publicIdService.GeneratePublicId();
        var createdAt = DateTime.UtcNow;
        var exerciseSet = new ExerciseSet
        {
            PublicId = publicId,
            StudentId = studentId,
            MinValue = minValue,
            MaxValue = maxValue,
            ExerciseCount = count,
            OperandCount = operandCount,
            OperationType = operationType,
            CreatedAt = createdAt,
            Exercises = exercises
        };

        await exerciseRepository.CreateExerciseSetAsync(exerciseSet);

        var exerciseViewModelList = CreateExerciseViewModelList(exercises);
        return new(publicId, exerciseViewModelList, createdAt, operandCount);
    }

    /// <summary>
    /// Generates a list of exercises based on the specified parameters.
    /// </summary>
    /// <param name="minValue">The minimum numeric value for operands in the generated exercises.</param>
    /// <param name="maxValue">The maximum numeric value for operands in the generated exercises.</param>
    /// <param name="count">The number of exercises to generate.</param>
    /// <param name="operandCount">The number of operands in each exercise.</param>
    /// <param name="possibleOperations">An array of possible operations allowed in the exercises.</param>
    /// <returns>A list of generated exercises.</returns>
    private static List<Exercise> GenerateExercisesList(
        int minValue,
        int maxValue,
        int count,
        int operandCount,
        SingleOperation[] possibleOperations)
    {
        var exercises = new List<Exercise>(count);
        for (int i = 0; i < count; i++)
        {
            int maxAttempts = 10;
            Exercise exercise;
            do
            {
                exercise = GenerateValidExercise(minValue, maxValue, operandCount, possibleOperations);    
            } while (exercises.Contains(exercise) && maxAttempts-- > 0);
            
            exercise.OrderIndex = i;
            exercises.Add(exercise);
        }

        return exercises;
    }

    /// <summary>
    /// Creates a list of exercise view models from the given list of exercises.
    /// </summary>
    /// <param name="exercises">The collection of exercises to convert into view models.</param>
    /// <returns>A list of exercise view models ordered by their order index.</returns>
    private static List<IExerciseViewModel> CreateExerciseViewModelList(List<Exercise> exercises)
    {
        return exercises
            .OrderBy(e => e.OrderIndex)
            .Select(ExerciseViewModel.Of)
            .ToList();
    }

    /// <summary>
    /// Generates a valid exercise by creating an exercise with random operands and operators
    /// within a specified range and validating its result to meet the required constraints.
    /// </summary>
    /// <param name="minValue">The minimum value for the operands in the exercise.</param>
    /// <param name="maxValue">The maximum value for the operands in the exercise.</param>
    /// <param name="operandCount">The number of operands used in the exercise.</param>
    /// <param name="possibleOperations">The range of mathematical operations allowed in the exercise.</param>
    /// <returns>An instance of the <see cref="Exercise"/> class representing a valid exercise.</returns>
    private static Exercise GenerateValidExercise(
        int minValue,
        int maxValue,
        int operandCount,
        SingleOperation[] possibleOperations)
    {
        const int maxAttempts = 100;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            var exercise = GenerateExercise(minValue, maxValue, operandCount, possibleOperations);

            // Validate the result
            try
            {
                var result = exercise.CalculateResult();

                // Check for valid values (no overflow, not negative for pure additive/subtractive operations)
                if (result >= 0 && result <= int.MaxValue / 2)
                {
                    return exercise;
                }
            }
            catch
            {
                // Division by zero or other errors - retry
            }
        }

        // Fallback: Simple addition
        var fallback = new Exercise
        {
            Operands =
            [
                ..Enumerable.Range(0, operandCount).Select(_ => RandomNumberGenerator.GetInt32(minValue, maxValue + 1))
            ],
            Operators = [..Enumerable.Repeat(SingleOperation.Addition, operandCount - 1)]
        };
        return fallback;
    }

    /// <summary>
    /// Generates a valid exercise with a specified number of operands and operations, within the given range.
    /// Depending on the operations provided, ensures that operands and results adhere to arithmetic rules
    /// (e.g., avoiding invalid division results or adhering to operator precedence in mixed operations).
    /// </summary>
    /// <param name="minValue">The minimum allowable value for generated operands.</param>
    /// <param name="maxValue">The maximum allowable value for generated operands.</param>
    /// <param name="operandCount">The number of operands in the generated exercise.</param>
    /// <param name="possibleOperations">An array of allowed operations to be used in the exercise.</param>
    /// <returns>An <see cref="Exercise"/> instance containing the generated operands and operators.</returns>
    private static Exercise GenerateExercise(
        int minValue,
        int maxValue,
        int operandCount,
        SingleOperation[] possibleOperations)
    {
        var operands = new List<int>();
        var operators = new List<SingleOperation>();

        // Check if mixed operations are present
        bool hasMultiplicationDivision = possibleOperations.Contains(SingleOperation.Multiplication) ||
                                         possibleOperations.Contains(SingleOperation.Division);
        bool hasAdditionOrSubtraction = possibleOperations.Contains(SingleOperation.Addition) ||
                                        possibleOperations.Contains(SingleOperation.Subtraction);
        bool isMixed = hasMultiplicationDivision && hasAdditionOrSubtraction;

        // For mixed operations: Adjust strategy
        if (isMixed && operandCount > 2)
        {
            return GenerateMixedExercise(minValue, maxValue, operandCount, possibleOperations);
        }

        // Generate first operand
        operands.Add(RandomNumberGenerator.GetInt32(Math.Max(1, minValue), maxValue + 1));

        for (int i = 1; i < operandCount; i++)
        {
            var operation = possibleOperations[RandomNumberGenerator.GetInt32(possibleOperations.Length)];
            operators.Add(operation);

            // Generate operands based on previous partial result
            var partialResult = Exercise.CalculateResult([..operands], [..operators.Take(operators.Count - 1)]);
            var nextOperand = GenerateOperandForOperation(operation, partialResult, minValue, maxValue);
            operands.Add(nextOperand);
        }

        return new Exercise { Operands = [..operands], Operators = [..operators] };
    }

    /// <summary>
    /// Generates a mixed mathematical exercise by combining random operands and operations,
    /// including addition, subtraction, multiplication, and division, while correctly
    /// handling operator precedence rules.
    /// </summary>
    /// <param name="minValue">The minimum value for the randomly generated operands.</param>
    /// <param name="maxValue">The maximum value for the randomly generated operands.</param>
    /// <param name="operandCount">The number of operands to include in the exercise.</param>
    /// <param name="possibleOperations">The array of operations from which the exercise will be constructed.</param>
    /// <returns>An <see cref="Exercise"/> object containing the generated operands and operators.</returns>
    private static Exercise GenerateMixedExercise(
        int minValue,
        int maxValue,
        int operandCount,
        SingleOperation[] possibleOperations)
    {
        var operands = new List<int>();
        var operators = new List<SingleOperation>();

        // Strategy: First generate all operands and operators,
        // then validate if the result is valid
        for (int i = 0; i < operandCount; i++)
        {
            // For division: Use smaller numbers to facilitate divisibility
            int operand;
            if (i > 0 && operators.Count > 0 && operators[^1] == SingleOperation.Division)
            {
                // For division after an operand: Find a divisor
                var prevOperand = operands[^1];
                var divisors = FindDivisors(prevOperand, Math.Max(1, minValue), Math.Min(prevOperand, maxValue));
                operand = divisors.Count > 0
                    ? divisors[RandomNumberGenerator.GetInt32(divisors.Count)]
                    : 1;
            }
            else
            {
                operand = RandomNumberGenerator.GetInt32(Math.Max(1, minValue), maxValue + 1);
            }

            operands.Add(operand);

            if (i >= operandCount - 1)
            {
                continue;
            }

            var operation = possibleOperations[RandomNumberGenerator.GetInt32(possibleOperations.Length)];

            // For division: Ensure the next operand can be a divisor
            if (operation == SingleOperation.Division)
            {
                // Adjust the current operand to enable divisibility
                var currentOperand = operands[^1];
                var possibleNextValues = FindDivisors(currentOperand, Math.Max(1, minValue), maxValue);
                if (possibleNextValues.Count == 0)
                {
                    // Fallback: Multiplication instead of division
                    operation = SingleOperation.Multiplication;
                }
            }

            operators.Add(operation);
        }

        return new Exercise
        {
            Operands = [..operands],
            Operators = [..operators]
        };
    }

    /// <summary>
    /// Generates an operand based on the operation.
    /// </summary>
    private static int GenerateOperandForOperation(
        SingleOperation operation,
        int currentResult,
        int minValue,
        int maxValue)
    {
        switch (operation)
        {
            case SingleOperation.Division:
                if (currentResult == 0)
                {
                    return 1;
                }

                var divisors = FindDivisors(Math.Abs(currentResult), Math.Max(1, minValue), maxValue);
                if (divisors.Count == 0)
                {
                    return 1;
                }

                return divisors[RandomNumberGenerator.GetInt32(divisors.Count)];

            case SingleOperation.Subtraction:
                var maxSubtract = Math.Min(maxValue, Math.Max(0, currentResult));
                if (maxSubtract < minValue)
                {
                    return Math.Max(0, minValue);
                }

                return RandomNumberGenerator.GetInt32(Math.Max(0, minValue), maxSubtract + 1);

            default:
                return RandomNumberGenerator.GetInt32(Math.Max(1, minValue), maxValue + 1);
        }
    }

    /// <summary>
    /// Finds all divisors of a number within the given range.
    /// </summary>
    private static List<int> FindDivisors(int number, int min, int max)
    {
        var divisors = new List<int>();
        if (number == 0)
        {
            return divisors;
        }

        for (int i = Math.Max(1, min); i <= Math.Min(Math.Abs(number), max); i++)
        {
            if (number % i == 0)
            {
                divisors.Add(i);
            }
        }

        return divisors;
    }
}