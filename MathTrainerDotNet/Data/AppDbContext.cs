using MathTrainerDotNet.Models;
using Microsoft.EntityFrameworkCore;

namespace MathTrainerDotNet.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    /// <summary>
    /// Gets or sets the database set of students.
    /// This property allows access to the collection of <see cref="Student"/> entities
    /// stored in the database, enabling query and manipulation of student records.
    /// </summary>
    public DbSet<Student> Students => this.Set<Student>();

    /// <summary>
    /// Gets or sets the database set of exercise sets.
    /// This property provides access to the collection of <see cref="ExerciseSet"/> entities
    /// stored in the database, enabling query and management of exercise set records.
    /// </summary>
    public DbSet<ExerciseSet> ExerciseSets => this.Set<ExerciseSet>();

    /// <summary>
    /// Gets or sets the database set of exercises.
    /// This property provides access to the collection of <see cref="Exercise"/> entities,
    /// allowing query and manipulation of exercise records stored in the database.
    /// </summary>
    public DbSet<Exercise> Exercises => this.Set<Exercise>();

    /// <summary>
    /// Configures the entity models and their relationships for the database using the provided <see cref="ModelBuilder"/>.
    /// This method is invoked when the model for a derived context is being created. It is used to define the schema required
    /// for the database, including specifying keys, relationships, property constraints, and other configurations.
    /// </summary>
    /// <param name="modelBuilder">An instance of <see cref="ModelBuilder"/>
    /// that is used to configure the entity models.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(student => student.Id);
            entity.HasIndex(student => student.Name).IsUnique();
            entity.Property(student => student.Name).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<ExerciseSet>(entity =>
        {
            entity.HasKey(exerciseSet => exerciseSet.Id);
            entity.HasIndex(exerciseSet => exerciseSet.PublicId).IsUnique();
            entity.Property(exerciseSet => exerciseSet.PublicId).HasMaxLength(10).IsRequired();
            entity.HasOne(exerciseSet => exerciseSet.Student)
                .WithMany(student => student.ExerciseSets)
                .HasForeignKey(exerciseSet => exerciseSet.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Exercise>(entity =>
        {
            entity.HasKey(exercise => exercise.Id);
            entity.HasOne(exercise => exercise.ExerciseSet)
                .WithMany(exerciseSet => exerciseSet.Exercises)
                .HasForeignKey(exercise => exercise.ExerciseSetId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.Property(exercise => exercise.OperandsJson).HasMaxLength(500);
            entity.Property(exercise => exercise.OperatorsJson).HasMaxLength(500);
        });
    }
}