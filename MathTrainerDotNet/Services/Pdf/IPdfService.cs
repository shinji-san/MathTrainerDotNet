namespace MathTrainerDotNet.Services.Pdf;

using Models;
using ViewModels;

/// <summary>
/// Provides functionality to generate PDF documents for math exercises and solutions.
/// </summary>
public interface IPdfService
{
    byte[] GenerateExercisePdf(
        string publicId,
        List<IExerciseViewModel> exercises,
        int minValue,
        int maxValue,
        OperationType operationType,
        string studentName,
        DateTime createdAt);

    byte[] GenerateSolutionPdf(
        string publicId,
        List<IExerciseViewModel> exercises,
        int minValue,
        int maxValue,
        OperationType operationType,
        string studentName,
        DateTime createdAt);

    string GenerateFileName(
        string publicId,
        OperationType operationType,
        DateTime createdAt,
        string studentName,
        bool isSolution = false);
}