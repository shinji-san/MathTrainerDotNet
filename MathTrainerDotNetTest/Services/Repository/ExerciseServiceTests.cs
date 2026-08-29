namespace MathTrainerDotNetTest.Services.Repository;

public class ExerciseRepositoryTests : IDisposable
{
    private readonly AppDbContext appDbContext;
    private readonly ExerciseRepository exerciseRepository;
    
    public ExerciseRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        this.appDbContext = new AppDbContext(options);
        this.exerciseRepository = new ExerciseRepository(this.appDbContext);

        // Seed a test student
        this.appDbContext.Students.Add(new Student { Id = 1, Name = "Test Student" });
        this.appDbContext.SaveChanges();
    }
    
    public void Dispose()
    {
        this.appDbContext.Database.EnsureDeleted();
        this.appDbContext.Dispose();
    }

    [Fact]
    public async Task CreateExerciseSetAsync_ShouldSaveToDatabase()
    {
        // Arrange
        const string publicId = "TEST01";
        var exerciseSet = new ExerciseSet
        {
            PublicId = publicId,
            StudentId = 1,
            MinValue = 1,
            MaxValue = 10,
            ExerciseCount = 2,
            OperationType = OperationType.Addition,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        await this.exerciseRepository.CreateExerciseSetAsync(exerciseSet);

        // Assert
        var savedSet = await this.appDbContext.ExerciseSets.FirstOrDefaultAsync(es => es.PublicId == publicId, TestContext.Current.CancellationToken);
        Assert.NotNull(savedSet);
        Assert.Equal(1, savedSet.StudentId);
    }

    [Fact]
    public async Task LoadExercisesByIdAsync_ExistingId_ShouldReturnCorrectData()
    {
        // Arrange
        const string publicId = "LOAD01";
        var exerciseSet = new ExerciseSet
        {
            PublicId = publicId,
            StudentId = 1,
            MinValue = 5,
            MaxValue = 15,
            OperationType = OperationType.Multiplication,
            CreatedAt = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc),
            OperandCount = 2,
            Exercises = new List<Exercise>
            {
                new() { OrderIndex = 2, Operands = [2, 3], Operators = [SingleOperation.Multiplication] },
                new() { OrderIndex = 1, Operands = [4, 5], Operators = [SingleOperation.Multiplication] }
            }
        };
        this.appDbContext.ExerciseSets.Add(exerciseSet);
        await this.appDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var result = await this.exerciseRepository.LoadExercisesByIdAsync(publicId);

        // Assert
        Assert.True(result.Found);
        Assert.Equal(5, result.MinValue);
        Assert.Equal(15, result.MaxValue);
        Assert.Equal(OperationType.Multiplication, result.OperationType);
        Assert.Equal("Test Student", result.StudentName);
        Assert.Equal(exerciseSet.CreatedAt, result.CreatedAt);
        Assert.Equal(2, result.OperandCount);
        Assert.Equal(2, result.Exercises.Count);
        
        // Test ordering
        Assert.Equal("4 × 5 = ", result.Exercises[0].DisplayText);
        Assert.Equal("2 × 3 = ", result.Exercises[1].DisplayText);
    }

    [Fact]
    public async Task LoadExercisesByIdAsync_CaseInsensitive_ShouldStillFind()
    {
        // Arrange
        var publicId = "CASE01";
        var exerciseSet = new ExerciseSet
        {
            PublicId = publicId, // ExerciseRepository uses ToUpperInvariant() on the input
            StudentId = 1,
            Exercises = new List<Exercise>()
        };
        this.appDbContext.ExerciseSets.Add(exerciseSet);
        await this.appDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var result = await this.exerciseRepository.LoadExercisesByIdAsync("case01");

        // Assert
        Assert.True(result.Found);
    }

    [Fact]
    public async Task LoadExercisesByIdAsync_NonExistingId_ShouldReturnFoundFalse()
    {
        // Act
        var result = await this.exerciseRepository.LoadExercisesByIdAsync("NONEXISTENT");

        // Assert
        Assert.False(result.Found);
    }
}
