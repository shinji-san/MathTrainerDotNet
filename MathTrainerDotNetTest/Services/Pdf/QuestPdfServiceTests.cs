namespace MathTrainerDotNetTest.Services.Pdf;

using MathTrainerDotNet.Services.Format;

public class QuestPdfServiceTests
{
    private readonly QuestPdfService questPdfService;
    private readonly ILocalizationService localizationService;

    public QuestPdfServiceTests()
    {
        this.localizationService = new LocalizationService();
        var dateFormatter = new DateFormatterServiceService();
        this.questPdfService = new QuestPdfService(this.localizationService, dateFormatter);
    }

    private static Exercise CreateExercise(List<int> operands, List<SingleOperation> operators)
    {
        var exercise = new Exercise
        {
            Operands = [..operands],
            Operators = [..operators]
        };

        return exercise;
    }

    private static List<IExerciseViewModel> CreateSampleExercises()
    {
        return
        [
            new ExerciseViewModel(CreateExercise([5, 3], [SingleOperation.Addition])),
            new ExerciseViewModel(CreateExercise([10, 4], [SingleOperation.Subtraction])),
            new ExerciseViewModel(CreateExercise([6, 7], [SingleOperation.Multiplication])),
            new ExerciseViewModel(CreateExercise([12, 3], [SingleOperation.Division]))
        ];
    }

    [Fact]
    public void GenerateExercisePdf_ShouldReturnNonEmptyByteArray()
    {
        // Arrange
        var exercises = CreateSampleExercises();

        // Act
        var result = this.questPdfService.GenerateExercisePdf(
            "ABC123",
            exercises,
            1,
            10,
            OperationType.All,
            "Test Student",
            DateTime.Now);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Length > 0);
    }

    [Fact]
    public void GenerateExercisePdf_ShouldStartWithPdfHeader()
    {
        // Arrange
        var exercises = CreateSampleExercises();

        // Act
        var result = this.questPdfService.GenerateExercisePdf(
            "ABC123",
            exercises,
            1,
            10,
            OperationType.All,
            "Test Student",
            DateTime.Now);

        // Assert - PDF files start with %PDF-
        Assert.Equal((byte)'%', result[0]);
        Assert.Equal((byte)'P', result[1]);
        Assert.Equal((byte)'D', result[2]);
        Assert.Equal((byte)'F', result[3]);
    }

    [Fact]
    public void GenerateSolutionPdf_ShouldReturnNonEmptyByteArray()
    {
        // Arrange
        var exercises = CreateSampleExercises();

        // Act
        var result = this.questPdfService.GenerateSolutionPdf(
            "ABC123",
            exercises,
            1,
            10,
            OperationType.All,
            "Test Student",
            DateTime.Now);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Length > 0);
    }

    [Fact]
    public void GenerateSolutionPdf_ShouldStartWithPdfHeader()
    {
        // Arrange
        var exercises = CreateSampleExercises();

        // Act
        var result = this.questPdfService.GenerateSolutionPdf(
            "ABC123",
            exercises,
            1,
            10,
            OperationType.All,
            "Test Student",
            DateTime.Now);

        // Assert
        Assert.Equal((byte)'%', result[0]);
        Assert.Equal((byte)'P', result[1]);
        Assert.Equal((byte)'D', result[2]);
        Assert.Equal((byte)'F', result[3]);
    }

    [Theory]
    [InlineData(OperationType.Multiplication, "de", "Multiplikation")]
    [InlineData(OperationType.Division, "de", "Division")]
    [InlineData(OperationType.Addition, "de", "Addition")]
    [InlineData(OperationType.Subtraction, "de", "Subtraktion")]
    [InlineData(OperationType.MultiplicationDivision, "de", "Punktrechenarten")]
    [InlineData(OperationType.AdditionSubtraction, "de", "Strichrechenarten")]
    [InlineData(OperationType.All, "de", "Alle")]
    [InlineData(OperationType.Multiplication, "en", "Multiplication")]
    [InlineData(OperationType.Division, "en", "Division")]
    [InlineData(OperationType.Addition, "en", "Addition")]
    [InlineData(OperationType.Subtraction, "en", "Subtraction")]
    [InlineData(OperationType.MultiplicationDivision, "en", "Multiplication_and_Division")]
    [InlineData(OperationType.AdditionSubtraction, "en", "Addition_and_Subtraction")]
    [InlineData(OperationType.All, "en", "All")]
    public void GenerateFileName_ShouldIncludeOperationType(OperationType operationType, string language, string expectedOperationName)
    {
        // Arrange
        this.localizationService.SetLanguage(language);
        var createdAt = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Local);

        // Act
        var fileName = this.questPdfService.GenerateFileName(
            "ABC123",
            operationType,
            createdAt,
            "Test Student");

        // Assert
        Assert.Contains(expectedOperationName, fileName);
    }

    [Fact]
    public void GenerateFileName_ShouldIncludeDate()
    {
        // Arrange
        var createdAt = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Local);

        // Act
        var fileName = this.questPdfService.GenerateFileName(
            "ABC123",
            OperationType.Addition,
            createdAt,
            "Test Student");

        // Assert
        Assert.Contains("2024-01-15_10-30-00", fileName);
    }

    [Fact]
    public void GenerateFileName_ShouldIncludePublicId()
    {
        // Arrange
        var createdAt = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Local);

        // Act
        var fileName = this.questPdfService.GenerateFileName(
            "XYZ789",
            OperationType.Addition,
            createdAt,
            "Test Student");

        // Assert
        Assert.Contains("XYZ789", fileName);
    }

    [Fact]
    public void GenerateFileName_ShouldEndWithPdf()
    {
        // Act
        var fileName = this.questPdfService.GenerateFileName(
            "ABC123",
            OperationType.Addition,
            DateTime.Now,
            "Test Student");

        // Assert
        Assert.EndsWith(".pdf", fileName);
    }

    [Theory]
    [InlineData("de")]
    [InlineData("en")]
    public void GenerateFileName_Solution_ShouldIncludePdfFileNameSolution(string language)
    {
        // Arrange
        this.localizationService.SetLanguage(language);

        // Act
        var fileName = this.questPdfService.GenerateFileName(
                "ABC123",
                OperationType.Addition,
                DateTime.Now,
                "Test Student",
                isSolution: true);

        // Assert
        Assert.Contains(this.localizationService["PdfFileNameSolution"], fileName);
    }

    [Theory]
    [InlineData("de")]
    [InlineData("en")]
    public void GenerateFileName_NotSolution_ShouldNotIncludePdfFileNameSolution(string language)
    {
        // Arrange
        this.localizationService.SetLanguage(language);

        // Act
        var fileName = this.questPdfService.GenerateFileName(
            "ABC123",
            OperationType.Addition,
            DateTime.Now,
            "Test Student",
            isSolution: false);

        // Assert
        Assert.DoesNotContain(this.localizationService["PdfFileNameSolution"], fileName);
    }

    [Fact]
    public void GenerateFileName_ShouldNormalizeStudentName()
    {
        // Arrange
        var createdAt = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Local);
        const string studentName = "  Müller-Lüdenscheidt \t Max  ";

        // Act
        var fileName = this.questPdfService.GenerateFileName(
            "ABC123",
            OperationType.Addition,
            createdAt,
            studentName);

        // Assert
        Assert.DoesNotContain(" ", fileName);
        Assert.DoesNotContain("\t", fileName);
        Assert.DoesNotContain("ü", fileName);
        Assert.Contains("Muller-Ludenscheidt-Max", fileName);
    }

    [Fact]
    public void GenerateExercisePdf_German_ShouldGenerateSuccessfully()
    {
        // Arrange
        this.localizationService.SetLanguage("de");
        var exercises = CreateSampleExercises();

        // Act
        var result = this.questPdfService.GenerateExercisePdf(
            "ABC123",
            exercises,
            1,
            10,
            OperationType.All,
            "Test Student",
            DateTime.Now);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Length > 100); // Should be a substantial PDF
    }

    [Fact]
    public void GenerateExercisePdf_English_ShouldGenerateSuccessfully()
    {
        // Arrange
        this.localizationService.SetLanguage("en");
        var exercises = CreateSampleExercises();

        // Act
        var result = this.questPdfService.GenerateExercisePdf(
            "ABC123",
            exercises,
            1,
            10,
            OperationType.Addition,
            "Test Student",
            DateTime.Now);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Length > 100);
    }

    [Fact]
    public void GeneratePdf_ManyExercises_ShouldGenerateSuccessfully()
    {
        // Arrange
        var exercises = new List<IExerciseViewModel>();
        for (int i = 0; i < 100; i++)
        {
            var exercise = CreateExercise([i + 1, i + 2], [SingleOperation.Addition]);
            var exerciseViewModel = new ExerciseViewModel(exercise);
            exercises.Add(exerciseViewModel);
        }

        // Act
        var result = this.questPdfService.GenerateExercisePdf(
            "ABC123",
            exercises,
            1,
            100,
            OperationType.Addition,
            "Test Student",
            DateTime.Now);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Length > 1000); // Should be a larger PDF with more pages
    }
}