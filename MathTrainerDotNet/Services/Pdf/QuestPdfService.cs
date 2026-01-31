namespace MathTrainerDotNet.Services.Pdf;

using Format;
using System.Text;
using System.Text.RegularExpressions;
using Localization;
using Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ViewModels;

public partial class QuestPdfService : IPdfService
{
    /// <summary>
    /// Represents a compiled regular expression that matches characters not in the set of letters, numbers, and hyphens.
    /// </summary>
    /// <returns>A <see cref="Regex"/> object created from the specified pattern.</returns>
    [GeneratedRegex(@"[^a-zA-Z0-9\-]")]
    private static partial Regex RemoveInvalidCharacters();

    /// <summary>
    /// Represents a compiled regular expression that matches one or more whitespace characters.
    /// </summary>
    /// <returns>A <see cref="Regex"/> object created from the specified pattern.</returns>
    [GeneratedRegex(@"\s+")]
    private static partial Regex RemoveWhiteSpaces();

    /// <summary>
    /// Provides access to localization functionality to support multi-language features within the PDF generation service.
    /// Used to retrieve localized strings and manage language-specific data.
    /// </summary>
    private readonly ILocalizationService localizationService;

    /// <summary>
    /// Handles date formatting operations within the PDF generation process.
    /// Used to transform DateTime values into culture-specific, human-readable string representations.
    /// </summary>
    private readonly IDateFormatterService dateFormatterService;

    /// <summary>
    /// Initializes a new instance of the <see cref="QuestPdfService"/> class.
    /// </summary>
    /// <param name="localizationService">The localization service.</param>
    /// <param name="dateFormatterService">The date formatter.</param>
    public QuestPdfService(ILocalizationService localizationService, IDateFormatterService dateFormatterService)
    {
        this.localizationService = localizationService;
        this.dateFormatterService = dateFormatterService;
        QuestPDF.Settings.License = LicenseType.Community;

    }

    public byte[] GenerateExercisePdf(
        string publicId,
        List<IExerciseViewModel> exercises,
        int minValue,
        int maxValue,
        OperationType operationType,
        string studentName,
        DateTime createdAt)
    {
        var title = this.GetPdfTitle(operationType, false);
        var rangeLabel = this.localizationService["NumberRange"];
        var nameLabel = this.localizationService["PdfName"];
        var pointsLabel = this.localizationService["PdfPoints"];
        var idHint = string.Format(this.localizationService["PdfEnterIdHint"], publicId);

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(14));

                page.Header().Element(header =>
                {
                    header.Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text(title)
                                .FontSize(22)
                                .Bold()
                                .FontColor(Colors.Blue.Darken2);

                            col.Item().Text($"{rangeLabel}: {minValue} - {maxValue}")
                                .FontSize(11)
                                .FontColor(Colors.Grey.Darken1);
                        });

                        row.ConstantItem(140).Column(col =>
                        {
                            col.Item().AlignRight().Text($"ID: {publicId}")
                                .FontSize(16)
                                .Bold()
                                .FontColor(Colors.Blue.Darken3);

                            var formattedDate =
                                this.dateFormatterService.Format(createdAt, this.localizationService.CurrentCulture);
                            col.Item().AlignRight().Text(formattedDate)
                                .FontSize(10)
                                .FontColor(Colors.Grey.Medium);
                        });
                    });
                });

                page.Content().PaddingVertical(15).Column(column =>
                {
                    column.Spacing(6);

                    column.Item().PaddingBottom(8).Row(row =>
                    {
                        row.RelativeItem().Text(text =>
                        {
                            text.Span($"{nameLabel}: ").FontSize(12);
                            text.Span(studentName).Bold().FontSize(12).Underline();
                        });
                        row.ConstantItem(150).Text($"{pointsLabel}: _____ / {exercises.Count}")
                            .FontSize(12);
                    });

                    column.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                    column.Item().PaddingTop(8);

                    RenderExercises(column, exercises, false);
                });

                page.Footer().Row(row =>
                {
                    row.RelativeItem().AlignLeft().Text(text =>
                    {
                        text.DefaultTextStyle(x => x.FontSize(10).FontColor(Colors.Grey.Medium));
                        text.CurrentPageNumber();
                        text.Span(" / ");
                        text.TotalPages();
                    });

                    row.RelativeItem().AlignCenter().Text(text =>
                    {
                        text.DefaultTextStyle(x => x.FontSize(9));
                        text.Span(idHint);
                    });

                    row.ConstantItem(50);
                });
            });
        });

        // Todo: PDF-Meta Data
        // var documentMetadata = new DocumentMetadata
        // {
        //     Title = title,
        //     Author = "",
        //     Producer = this.localizationService["AppTitle"],
        //     Keywords = "math, exercises, solutions"
        // // };
        //
        // var fullDocument = document.WithMetadata(documentMetadata);
        // return fullDocument.GeneratePdf();
        return document.GeneratePdf();
    }

    public byte[] GenerateSolutionPdf(
        string publicId,
        List<IExerciseViewModel> exercises,
        int minValue,
        int maxValue,
        OperationType operationType,
        string studentName,
        DateTime createdAt)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var title = this.GetPdfTitle(operationType, true);
        var rangeLabel = this.localizationService["NumberRange"];
        var nameLabel = this.localizationService["PdfName"];
        var countLabel = this.localizationService["PdfCount"];
        var exercisesLabel = this.localizationService["PdfExercises"];
        var solutionSheetText = this.localizationService["PdfSolutionSheet"];

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(14));

                page.Header().Element(header =>
                {
                    header.Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text(title)
                                .FontSize(22)
                                .Bold()
                                .FontColor(Colors.Green.Darken2);

                            col.Item().Text($"{rangeLabel}: {minValue} - {maxValue}")
                                .FontSize(11)
                                .FontColor(Colors.Grey.Darken1);
                        });

                        row.ConstantItem(140).Column(col =>
                        {
                            col.Item().AlignRight().Text($"ID: {publicId}")
                                .FontSize(16)
                                .Bold()
                                .FontColor(Colors.Green.Darken3);

                            var formattedDate =
                                this.dateFormatterService.Format(createdAt, this.localizationService.CurrentCulture);
                            col.Item().AlignRight().Text(formattedDate)
                                .FontSize(10)
                                .FontColor(Colors.Grey.Medium);
                        });
                    });
                });

                page.Content().PaddingVertical(15).Column(column =>
                {
                    column.Spacing(6);

                    column.Item().PaddingBottom(8).Row(row =>
                    {
                        row.RelativeItem().Text(text =>
                        {
                            text.Span($"{nameLabel}: ").FontSize(12);
                            text.Span(studentName).Bold().FontSize(12).Underline();
                        });
                        row.ConstantItem(150).Text($"{countLabel}: {exercises.Count} {exercisesLabel}")
                            .FontSize(12);
                    });

                    column.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                    column.Item().PaddingTop(8);

                    RenderExercises(column, exercises, true);
                });

                page.Footer().Row(row =>
                {
                    row.RelativeItem().AlignLeft().Text(text =>
                    {
                        text.DefaultTextStyle(x => x.FontSize(10).FontColor(Colors.Grey.Medium));
                        text.CurrentPageNumber();
                        text.Span(" / ");
                        text.TotalPages();
                    });

                    row.RelativeItem().AlignCenter().Text(text =>
                    {
                        text.DefaultTextStyle(x => x.FontSize(9).FontColor(Colors.Green.Darken2));
                        text.Span(solutionSheetText);
                    });

                    row.ConstantItem(50);
                });
            });
        });

        return document.GeneratePdf();
    }

    private static void RenderExercises(ColumnDescriptor column, List<IExerciseViewModel> exercises, bool showSolutions)
    {
        var leftColumn = exercises.Take(exercises.Count / 2 + exercises.Count % 2).ToList();
        var rightColumn = exercises.Skip(exercises.Count / 2 + exercises.Count % 2).ToList();

        column.Item().Row(row =>
        {
            row.RelativeItem().Column(col =>
            {
                for (int i = 0; i < leftColumn.Count; i++)
                {
                    var exercise = leftColumn[i];
                    var number = i + 1;
                    RenderExerciseRow(col, number, exercise, showSolutions);
                }
            });

            row.ConstantItem(30);

            row.RelativeItem().Column(col =>
            {
                for (int i = 0; i < rightColumn.Count; i++)
                {
                    var exercise = rightColumn[i];
                    var number = leftColumn.Count + i + 1;
                    RenderExerciseRow(col, number, exercise, showSolutions);
                }
            });
        });
    }

    private static void RenderExerciseRow(ColumnDescriptor col, int number, IExerciseViewModel exercise,
        bool showSolution)
    {
        col.Item().PaddingBottom(10).Row(r =>
        {
            r.ConstantItem(28).Text($"{number}.")
                .FontSize(12)
                .FontColor(Colors.Grey.Darken1);

            r.RelativeItem().Text(exercise.DisplayText)
                .FontSize(13);

            if (showSolution)
            {
                r.ConstantItem(80).Text($"{exercise.CorrectResult}")
                    .FontSize(13)
                    .Bold()
                    .FontColor(Colors.Green.Darken2);
            }
            else
            {
                r.ConstantItem(80).Text("________")
                    .FontSize(13)
                    .FontColor(Colors.Grey.Lighten1);
            }
        });
    }

    private string GetPdfTitle(OperationType type, bool isSolution)
    {
        var suffix = isSolution ? " - " + this.localizationService["PdfSolutions"] : "";

        return type switch
        {
            OperationType.Multiplication => this.localizationService["Multiplication"] + suffix,
            OperationType.Division => this.localizationService["Division"] + suffix,
            OperationType.Addition => this.localizationService["Addition"] + suffix,
            OperationType.Subtraction => this.localizationService["Subtraction"] + suffix,
            OperationType.MultiplicationDivision => this.localizationService["MultiplicationDivision"] + suffix,
            OperationType.AdditionSubtraction => this.localizationService["AdditionSubtraction"] + suffix,
            OperationType.All => this.localizationService["AllOperations"] + suffix,
            _ => this.localizationService["AppTitle"] + suffix
        };
    }

    public string GenerateFileName(
        string publicId,
        OperationType operationType,
        DateTime createdAt,
        string studentName,
        bool isSolution = false)
    {
        var operationName = operationType.ToFileNameString(this.localizationService);
        var normalizedStudentName = NormalizeStudentName(studentName);
        var dateStr = createdAt.ToLocalTime().ToString("yyyy-MM-dd_HH-mm-ss");
        var suffix = isSolution ? this.localizationService["PdfFileNameSolution"] : "";
        var currentLanguage = this.localizationService.CurrentLanguage;
        return $"{currentLanguage}_{operationName}_{normalizedStudentName}_{dateStr}_{publicId}_{suffix}.pdf";
    }

    private static string NormalizeStudentName(string name)
    {
        var normalizedString = name.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        var result = stringBuilder.ToString().Normalize(NormalizationForm.FormC);

        result = RemoveWhiteSpaces().Replace(result, "-");
        result = RemoveInvalidCharacters().Replace(result, "");
        result = result.Trim('-');

        return result;
    }
}
