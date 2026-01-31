namespace MathTrainerDotNet.Services.Repository;

using Models;

/// <summary>
/// Represents a repository interface for managing exercise sets in the system.
/// Provides methods for creating and retrieving exercise sets.
/// </summary>
public interface IExerciseRepository
{
    /// <summary>
    /// Adds a new exercise set to the database and commits the changes asynchronously.
    /// </summary>
    /// <param name="exerciseSet">The exercise set to be added to the database.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task CreateExerciseSetAsync(ExerciseSet exerciseSet);

    /// <summary>
    /// Asynchronously loads an exercise set by its public ID from the database.
    /// </summary>
    /// <param name="publicId">The public identifier of the exercise set to be loaded.</param>
    /// <returns>A task representing the asynchronous operation, containing a <see cref="LoadExercisesResult"/> with
    /// the details of the exercise set if found.</returns>
    Task<LoadExercisesResult> LoadExercisesByIdAsync(string publicId);
}