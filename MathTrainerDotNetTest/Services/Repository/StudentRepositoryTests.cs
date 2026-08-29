namespace MathTrainerDotNetTest.Services.Repository;

public class StudentRepositoryTests : IDisposable
{
    private readonly AppDbContext appDbContext;
    private readonly StudentRepository repository;
    
    public StudentRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        this.appDbContext = new AppDbContext(options);
        this.repository = new StudentRepository(this.appDbContext);
    }
    
    public void Dispose()
    {
        this.appDbContext.Database.EnsureDeleted();
        this.appDbContext.Dispose();
    }
    
    [Fact]
    public async Task GetOrCreateStudentAsync_NewStudent_ShouldCreateStudent()
    {
        // Act
        var student = await this.repository.GetOrCreateStudentAsync("Test Student");
        
        // Assert
        Assert.NotNull(student);
        Assert.Equal("Test Student", student.Name);
        Assert.True(student.Id > 0);
    }
    
    [Fact]
    public async Task GetOrCreateStudentAsync_ExistingStudent_ShouldReturnExisting()
    {
        // Arrange
        var student1 = await this.repository.GetOrCreateStudentAsync("Test Student");
        
        // Act
        var student2 = await this.repository.GetOrCreateStudentAsync("Test Student");
        
        // Assert
        Assert.Equal(student1.Id, student2.Id);
    }
    
    [Fact]
    public async Task GetOrCreateStudentAsync_CaseSensitive_ShouldReturnSame()
    {
        // Act
        var student1 = await this.repository.GetOrCreateStudentAsync("Max");
        var student2 = await this.repository.GetOrCreateStudentAsync("max");
        
        // Assert
        Assert.Equal(student1.Id, student2.Id);
    }
    
    [Fact]
    public async Task GetOrCreateStudentAsync_ShouldTrimName()
    {
        // Act
        var student1 = await this.repository.GetOrCreateStudentAsync("  Max  ");
        var student2 = await this.repository.GetOrCreateStudentAsync("Max");
        
        // Assert
        Assert.Equal(student1.Id, student2.Id);
        Assert.Equal("Max", student1.Name);
    }
    
    [Fact]
    public async Task GetAllStudentsAsync_Empty_ShouldReturnEmptyList()
    {
        // Act
        var students = await this.repository.GetAsyncAllStudents().ToListAsync(TestContext.Current.CancellationToken);
        
        // Assert
        Assert.Empty(students);
    }
    
    [Fact]
    public async Task GetAllStudentsAsync_WithStudents_ShouldReturnAll()
    {
        // Arrange
        await this.repository.GetOrCreateStudentAsync("Anna");
        await this.repository.GetOrCreateStudentAsync("Ben");
        await this.repository.GetOrCreateStudentAsync("Clara");
        
        // Act
        var students = await this.repository.GetAsyncAllStudents().ToListAsync(TestContext.Current.CancellationToken);
        
        // Assert
        Assert.Equal(3, students.Count);
    }
    
    [Fact]
    public async Task GetAllStudentsAsync_ShouldBeSortedByName()
    {
        // Arrange
        await this.repository.GetOrCreateStudentAsync("Clara");
        await this.repository.GetOrCreateStudentAsync("Anna");
        await this.repository.GetOrCreateStudentAsync("Ben");
        
        // Act
        var students = await this.repository.GetAsyncAllStudents().ToListAsync(TestContext.Current.CancellationToken);
        
        // Assert
        Assert.Equal("Anna", students[0].Name);
        Assert.Equal("Ben", students[1].Name);
        Assert.Equal("Clara", students[2].Name);
    }
    
    [Fact]
    public async Task GetStudentByIdAsync_ExistingId_ShouldReturnStudent()
    {
        // Arrange
        var created = await this.repository.GetOrCreateStudentAsync("Max");
        
        // Act
        var student = await this.repository.GetStudentByIdAsync(created.Id);
        
        // Assert
        Assert.NotNull(student);
        Assert.Equal("Max", student.Name);
    }
    
    [Fact]
    public async Task GetStudentByIdAsync_NonExistingId_ShouldReturnNull()
    {
        // Act
        var student = await this.repository.GetStudentByIdAsync(9999);
        
        // Assert
        Assert.Null(student);
    }
}
