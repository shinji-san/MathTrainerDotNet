namespace MathTrainerDotNet.Services.Repository;

using Data;
using Microsoft.EntityFrameworkCore;
using Models;
using ViewModels;

/// <summary>
/// Provides services for managing exercises, including generating new exercise sets and retrieving existing ones.
/// </summary>
public sealed class ExerciseRepository : IExerciseRepository
{
    /// <summary>
    /// Represents the database context for accessing and managing persistent data
    /// related to the application's domain objects, such as students, exercise sets,
    /// and exercises. Provides functionality to query, add, update, and delete records
    /// in the database via entity sets.
    /// </summary>
    private readonly AppDbContext appDbContext;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ExerciseRepository"/> class.
    /// </summary>
    /// <param name="appDbContext">The database context for accessing and managing persistent data.</param>
    public ExerciseRepository(AppDbContext appDbContext)
    {
        this.appDbContext = appDbContext;
    }

    /// <inheritdoc/>
    public async Task CreateExerciseSetAsync(ExerciseSet exerciseSet)
    {
        this.appDbContext.ExerciseSets.Add(exerciseSet);
        await this.appDbContext.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task<LoadExercisesResult> LoadExercisesByIdAsync(string publicId)
    {
        var exerciseSet = await this.appDbContext.ExerciseSets
            .Include(es => es.Exercises)
            .Include(es => es.Student)
            .FirstOrDefaultAsync(exerciseSetItem => exerciseSetItem.PublicId == publicId.ToUpperInvariant());

        if (exerciseSet == null)
        {
            return new LoadExercisesResult { Found = false };
        }

        var viewModels = exerciseSet.Exercises
            .OrderBy(e => e.OrderIndex)
            .Select(ExerciseViewModel.Of)
            .ToList();
        
        return new LoadExercisesResult
        {
            Found = true,
            Exercises = viewModels,
            MinValue = exerciseSet.MinValue,
            MaxValue = exerciseSet.MaxValue,
            OperationType = exerciseSet.OperationType,
            StudentName = exerciseSet.Student.Name,
            CreatedAt = exerciseSet.CreatedAt,
            OperandCount = exerciseSet.OperandCount
        };
    }
}