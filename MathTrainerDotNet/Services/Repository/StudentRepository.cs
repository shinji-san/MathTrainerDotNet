namespace MathTrainerDotNet.Services.Repository;

using Data;
using Microsoft.EntityFrameworkCore;
using Models;

/// <summary>
/// Provides functionality to manage student records in the database.
/// </summary>
public class StudentRepository : IStudentRepository
{
    private readonly AppDbContext appDbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="StudentRepository"/> class.
    /// </summary>
    /// <param name="appDbContext">The application database context.</param>
    public StudentRepository(AppDbContext appDbContext)
    {
        this.appDbContext = appDbContext;
    }

    /// <inheritdoc/>
    public IAsyncEnumerable<Student> GetAsyncAllStudents()
    {
        return this.appDbContext
            .Students
            .OrderBy(student => student.Name)
            .ToAsyncEnumerable();
    }

    /// <inheritdoc/>
    public async Task<Student?> GetStudentByNameAsync(string name)
    {
        return await this.appDbContext
            .Students
            .FirstOrDefaultAsync(student => student.Name.ToLower() == name.ToLower().Trim());
    }

    /// <inheritdoc/>
    public async Task<Student> GetOrCreateStudentAsync(string name)
    {
        name = name.Trim();
        var existing = await this.GetStudentByNameAsync(name);
        if (existing != null)
        {
            return existing;
        }

        var student = new Student { Name = name };
        this.appDbContext.Students.Add(student);
        await this.appDbContext.SaveChangesAsync();

        return student;
    }

    /// <inheritdoc/>
    public async Task<Student?> GetStudentByIdAsync(int id)
    {
        return await this.appDbContext.Students.FindAsync(id);
    }
}