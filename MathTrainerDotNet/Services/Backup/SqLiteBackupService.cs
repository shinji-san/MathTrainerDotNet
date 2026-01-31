namespace MathTrainerDotNet.Services.Backup;

using Data;
using Data.Helper;
using Localization;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Service for backup and restore of the SQLite database.
/// </summary>
public class SqLiteBackupService
{
    /// <summary>
    /// Provides access to a collection of application services, enabling dependency injection
    /// and scope creation for operations such as database context management and service usage
    /// within the <c>BackupService</c>.
    /// </summary>
    private readonly IServiceProvider serviceProvider;

    /// <summary>
    /// Provides localization functionality, including access to current language information,
    /// retrieval of localized strings, and language switching capabilities within the <c>BackupService</c>.
    /// </summary>
    private readonly ILocalizationService localizationService;

    /// <summary>
    /// Represents the file system path of the SQLite database used within the application.
    /// This value is initialized to the path of the database file, typically stored in the application's base directory.
    /// </summary>
    private readonly string databaseFilePath;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqLiteBackupService"/> class.
    /// </summary>
    /// <param name="serviceProvider">Service provider for dependency injection.</param>
    /// <param name="localizationService">Localization service for language-related operations.</param>
    /// <param name="configuration">Configuration settings for the application.</param>
    public SqLiteBackupService(
        IServiceProvider serviceProvider,
        ILocalizationService localizationService,
        IConfiguration configuration)
    {
        this.serviceProvider = serviceProvider;
        this.localizationService = localizationService;
        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? "Data Source=mathtrainer.db";
        this.databaseFilePath = DatabasePathHelper.GetDatabaseFilePath(connectionString);
    }

    /// <summary>
    /// Creates a backup of the database and returns the file contents as a byte array.
    /// Ensures that all changes are finalized and the database write-ahead log is cleared before generating the backup.
    /// </summary>
    /// <returns>A byte array containing the contents of the database file.</returns>
    /// <exception cref="FileNotFoundException">Thrown if the database file is not found at the specified path.</exception>
    public async Task<byte[]> CreateBackupAsync()
    {
        // Ensure all changes are written
        using var scope = this.serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Execute checkpoint to clear WAL file
        await context.Database.ExecuteSqlRawAsync("PRAGMA wal_checkpoint(TRUNCATE);");

        // Read file
        if (!File.Exists(this.databaseFilePath))
        {
            throw new FileNotFoundException(this.localizationService["DatabaseFileNotFound"], this.databaseFilePath);
        }

        return await File.ReadAllBytesAsync(this.databaseFilePath);
    }

    /// <summary>
    /// Generates a backup file name based on the current date and time.
    /// </summary>
    /// <returns>A string representing the generated backup file name.</returns>
    public static string GenerateBackupFileName()
    {
        return $"mathtrainer_backup_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.db";
    }

    /// <summary>
    /// Restores a backup from the provided byte array data.
    /// </summary>
    /// <param name="backupData">The byte array containing the backup data to restore.</param>
    /// <returns>A <see cref="RestoreResult"/> indicating the success or failure of the operation.</returns>
    /// <exception cref="Exception">Thrown when an unexpected error occurs during the restore process.</exception>
    public async Task<RestoreResult> RestoreBackupAsync(byte[] backupData)
    {
        if (backupData.Length == 0)
        {
            return new RestoreResult(Success: false, Message: this.localizationService["NoDataReceived"]);
        }

        // Validate SQLite file (Magic Header: "SQLite format 3\0")
        if (!IsValidSqliteFile(backupData))
        {
            return new RestoreResult(Success: false, Message: this.localizationService["InvalidSQLiteFile"]);
        }

        var validationResult = await this.ValidateDatabaseStructureAsync(backupData);
        if (!validationResult.IsValid)
        {
            return new RestoreResult(Success: false, Message: validationResult.Message);
        }

        try
        {
            // Create temporary file
            var tempPath = Path.Combine(Path.GetTempPath(), $"mathtrainer_restore_{Guid.NewGuid()}.db");
            await File.WriteAllBytesAsync(tempPath, backupData);

            // Close all connections
            using (var scope = this.serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await context.Database.CloseConnectionAsync();
            }

            SqliteConnection.ClearAllPools();

            // Wait briefly until all handles are released
            await Task.Delay(200);

            // Create a backup of the current database (security)
            var currentBackupPath = this.databaseFilePath + ".bak";
            if (File.Exists(this.databaseFilePath))
            {
                File.Copy(this.databaseFilePath, currentBackupPath, overwrite: true);
            }

            try
            {
                DeleteDatabaseFile(this.databaseFilePath);

                //// WAL and SHM file must be deleted, if exists
                DeleteDatabaseFile(this.databaseFilePath + "-wal");
                DeleteDatabaseFile(this.databaseFilePath + "-shm");

                File.Move(tempPath, this.databaseFilePath);

                DeleteDatabaseFile(currentBackupPath);

                return new RestoreResult
                (
                    Success: true,
                    Message: string.Format(this.localizationService["RestoreSuccess"], validationResult.StudentCount, validationResult.ExerciseSetCount)
                );
            }
            catch (Exception ex)
            {
                // On error: Restore backup
                if (!File.Exists(currentBackupPath))
                {
                    var message = string.Format(this.localizationService["RestoreErrorGeneral"], ex.Message);
                    throw new Exception(message, ex);
                }

                DeleteDatabaseFile(this.databaseFilePath);

                File.Move(currentBackupPath, this.databaseFilePath);

                var errorMessage = string.Format(this.localizationService["RestoreErrorGeneral"], ex.Message);
                throw new Exception(errorMessage, ex);
            }
            finally
            {
                // Cleanup temporary file
                DeleteDatabaseFile(tempPath);
            }
        }
        catch (Exception ex)
        {
            return new RestoreResult(Success: false, Message: string.Format(this.localizationService["RestoreError"], ex.Message));
        }
    }

    /// <summary>
    /// Deletes the database file at the specified file path if it exists.
    /// </summary>
    /// <param name="databaseFilePath">The file path of the database to be deleted.</param>
    private static void DeleteDatabaseFile(string databaseFilePath)
    {
        if (File.Exists(databaseFilePath))
        {
            File.Delete(databaseFilePath);
        }
    }

    /// <summary>
    /// Verifies if the provided data represents a valid SQLite file.
    /// </summary>
    /// <param name="data">The byte array containing the file data to validate.</param>
    /// <returns>
    /// <see langword="true"/> if the data represents a valid SQLite file; otherwise, <see langword="false"/>.
    /// </returns>
    private static bool IsValidSqliteFile(byte[] data)
    {
        if (data.Length < 16)
        {
            return false;
        }

        //// SQLite Magic Header: "SQLite format 3\0"
        var sqliteHeader = "SQLite format 3\0"u8.ToArray();
        for (int i = 0; i < sqliteHeader.Length; i++)
        {
            if (data[i] != sqliteHeader[i])
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Validates the database structure.
    /// </summary>
    private async Task<ValidationResult> ValidateDatabaseStructureAsync(byte[] data)
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"mathtrainer_validate_{Guid.NewGuid()}.db");

        try
        {
            await File.WriteAllBytesAsync(tempPath, data);

            var connectionString = $"Data Source={tempPath}";
            await using var connection = new SqliteConnection(connectionString);
            await connection.OpenAsync();

            // Check if required tables exist
            string[] requiredTables = ["Students", "ExerciseSets", "Exercises"];

            foreach (var table in requiredTables)
            {
                await using var cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name=@tableName";
                cmd.Parameters.AddWithValue("@tableName", table);
                var result = await cmd.ExecuteScalarAsync();

                if (result == null)
                {
                    return new ValidationResult
                    {
                        IsValid = false,
                        Message = string.Format(this.localizationService["TableNotFound"], table)
                    };
                }
            }

            // Count entries
            int studentCount;
            int exerciseSetCount;

            await using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT COUNT(*) FROM Students";
                studentCount = Convert.ToInt32(await cmd.ExecuteScalarAsync());
            }

            await using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT COUNT(*) FROM ExerciseSets";
                exerciseSetCount = Convert.ToInt32(await cmd.ExecuteScalarAsync());
            }

            return new ValidationResult
            {
                IsValid = true,
                StudentCount = studentCount,
                ExerciseSetCount = exerciseSetCount
            };
        }
        catch (Exception ex)
        {
            return new ValidationResult
            {
                IsValid = false,
                Message = string.Format(this.localizationService["DatabaseValidationFailed"], ex.Message)
            };
        }
        finally
        {
            SqliteConnection.ClearAllPools();
            if (File.Exists(tempPath))
            {
                try
                {
                    File.Delete(tempPath);
                }
                catch
                {
                }
            }
        }
    }

    /// <summary>
    /// Returns statistics about the current database.
    /// </summary>
    public async Task<DatabaseStats> GetDatabaseStatsAsync()
    {
        using var scope = this.serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var stats = new DatabaseStats
        {
            StudentCount = await context.Students.CountAsync(),
            ExerciseSetCount = await context.ExerciseSets.CountAsync(),
            ExerciseCount = await context.Exercises.CountAsync()
        };

        if (!File.Exists(this.databaseFilePath))
        {
            return stats;
        }

        var fileInfo = new FileInfo(this.databaseFilePath);
        stats.FileSizeBytes = fileInfo.Length;
        stats.LastModified = fileInfo.LastWriteTime;

        return stats;
    }
}

public record RestoreResult(bool Success, string Message = "");

public class ValidationResult
{
    public bool IsValid { get; set; }
    public string Message { get; set; } = string.Empty;
    public int StudentCount { get; set; }
    public int ExerciseSetCount { get; set; }
}

public class DatabaseStats
{
    public int StudentCount { get; set; }
    public int ExerciseSetCount { get; set; }
    public int ExerciseCount { get; set; }
    public long FileSizeBytes { get; set; }
    public DateTime LastModified { get; set; }

    public string FileSizeFormatted => this.FileSizeBytes switch
    {
        < 1024 => $"{this.FileSizeBytes} B",
        < 1024 * 1024 => $"{this.FileSizeBytes / 1024.0:F1} KB",
        _ => $"{this.FileSizeBytes / (1024.0 * 1024.0):F2} MB"
    };
}