namespace MathTrainerDotNet.Services.Repository;

using Models;

/// <summary>
/// Defines methods for managing and retrieving student records.
/// </summary>
public interface IStudentRepository
{
    /// <summary>
    /// Returns all students sorted by name.
    /// </summary>
    IAsyncEnumerable<Student> GetAsyncAllStudents();

    /// <summary>
    /// Returns a student by their name.
    /// </summary>
    Task<Student?> GetStudentByNameAsync(string name);

    /// <summary>
    /// Erstellt einen neuen Schüler oder gibt den bestehenden zurück.
    /// </summary>
    Task<Student> GetOrCreateStudentAsync(string name);

    /// <summary>
    /// Returns a student by their ID.
    /// </summary>
    Task<Student?> GetStudentByIdAsync(int id);
}