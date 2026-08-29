namespace MathTrainerDotNetTest.Services;

public class ExerciseGeneratorServiceTests : IDisposable
{
    private const string TestStudentName = "Test Student";
    private readonly AppDbContext appDbContext;
    private readonly ExerciseGeneratorService exerciseService;
    
    public ExerciseGeneratorServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        this.appDbContext = new AppDbContext(options);
        var exerciseRepository = new ExerciseRepository(this.appDbContext);
        this.exerciseService = new ExerciseGeneratorService(exerciseRepository, new PublicIdService());

        // Seed a test student
        this.appDbContext.Students.Add(new Student { Id = 1, Name =  TestStudentName});
        this.appDbContext.SaveChanges();
    }
    
    public void Dispose()
    {
        this.appDbContext.Database.EnsureDeleted();
        this.appDbContext.Dispose();
    }
    
    [Fact]
    public async Task GenerateExercisesAsync_ShouldGenerateCorrectNumberOfExercises()
    {
        // Arrange
        const int exerciseCount = 10;
        
        // Act
        var (_, exercises, _, _) = await this.exerciseService.GenerateExercisesAsync(
            studentId: 1,
            minValue: 1,
            maxValue: 10,
            count: exerciseCount,
            operationType: OperationType.Addition,
            operandCount: 2);
        
        // Assert
        Assert.Equal(exerciseCount, exercises.Count());
    }
    
    [Fact]
    public async Task GenerateExercisesAsync_ShouldGenerateUniquePublicId()
    {
        // Act
        var (publicId1, _, _, _) = await this.exerciseService.GenerateExercisesAsync(1, 1, 10, 5, OperationType.Addition);
        var (publicId2, _, _, _) = await this.exerciseService.GenerateExercisesAsync(1, 1, 10, 5, OperationType.Addition);
        
        // Assert
        Assert.NotEqual(publicId1, publicId2);
        Assert.Equal(6, publicId1.Length);
        Assert.Equal(6, publicId2.Length);
    }
    
    [Fact]
    public async Task GenerateExercisesAsync_ShouldSaveToDatabase()
    {
        // Act
        var (publicId, _, _, _) = await this.exerciseService.GenerateExercisesAsync(1, 1, 10, 5, OperationType.Addition);
        
        // Assert
        var savedSet = await this.appDbContext.ExerciseSets
            .Include(es => es.Exercises)
            .FirstOrDefaultAsync(es => es.PublicId == publicId, TestContext.Current.CancellationToken);
        
        Assert.NotNull(savedSet);
        Assert.Equal(5, savedSet.Exercises.Count());
    }
    
    [Theory]
    [InlineData(OperationType.Addition)]
    [InlineData(OperationType.Subtraction)]
    [InlineData(OperationType.Multiplication)]
    [InlineData(OperationType.Division)]
    public async Task GenerateExercisesAsync_ShouldGenerateCorrectOperationType(OperationType operationType)
    {
        // Act
        var (_, exercises, _, _) = await this.exerciseService.GenerateExercisesAsync(
            1, 1, 10, 10, operationType, 2);
        
        // Assert
        var possibleOps = operationType.GetPossibleOperations();
        foreach (var exercise in exercises)
        {
            Assert.All(exercise.Operators, op => Assert.Contains(op, possibleOps));
        }
    }
    
    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public async Task GenerateExercisesAsync_ShouldGenerateCorrectOperandCount(int operandCount)
    {
        // Act
        var (_, exercises, _, returnedOperandCount) = await this.exerciseService.GenerateExercisesAsync(
            1, 1, 10, 10, OperationType.Addition, operandCount);
        
        // Assert
        Assert.Equal(operandCount, returnedOperandCount);
        foreach (var exercise in exercises)
        {
            Assert.Equal(operandCount, exercise.Operands.Count());
            Assert.Equal(operandCount - 1, exercise.Operators.Count());
        }
    }
    
    [Fact]
    public async Task GenerateExercisesAsync_Division_ShouldProduceIntegerResults()
    {
        // Act
        var (_, exercises, _, _) = await this.exerciseService.GenerateExercisesAsync(
            1, 1, 20, 50, OperationType.Division, 2);
        
        // Assert
        foreach (var exercise in exercises)
        {
            var result = exercise.CorrectResult;
            Assert.True(result == (int)result, 
                $"Division result {result} should be an integer");
        }
    }
    
    [Fact]
    public async Task GenerateExercisesAsync_Subtraction_ShouldProduceNonNegativeResults()
    {
        // Act
        var (_, exercises, _, _) = await this.exerciseService.GenerateExercisesAsync(
            1, 1, 10, 50, OperationType.Subtraction, 2);
        
        // Assert
        foreach (var exercise in exercises)
        {
            Assert.True(exercise.CorrectResult >= 0, 
                $"Subtraction result {exercise.CorrectResult} should be non-negative");
        }
    }

    [Fact]
    public async Task GenerateExercisesAsync_MinOperandCountIsTwo()
    {
        // Act - trying to pass 1 operand (should be corrected to 2)
        var (_, exercises, _, returnedOperandCount) = await this.exerciseService.GenerateExercisesAsync(
            1, 1, 10, 5, OperationType.Addition, 1);
        
        // Assert
        Assert.Equal(2, returnedOperandCount);
        foreach (var exercise in exercises)
        {
            Assert.True(exercise.Operands.Count() >= 2);
        }
    }
}
