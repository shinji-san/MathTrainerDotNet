namespace MathTrainerDotNet.Data.Helper;

/// <summary>
/// Helper class for resolving SQLite database paths.
/// </summary>
public static class DatabasePathHelper
{
    /// <summary>
    /// Resolves the absolute database file path from a connection string.
    /// If the path is relative, it is combined with the application's base directory.
    /// </summary>
    /// <param name="connectionString">The SQLite connection string (e.g., "Data Source=mathtrainer.db").</param>
    /// <returns>The absolute path to the database file.</returns>
    public static string GetDatabaseFilePath(string connectionString)
    {
        var dbPath = connectionString.Replace("Data Source=", "", StringComparison.OrdinalIgnoreCase).Trim();
        if (!Path.IsPathRooted(dbPath))
        {
            dbPath = Path.Combine(AppContext.BaseDirectory, dbPath);
        }
        return dbPath;
    }

    /// <summary>
    /// Resolves a full SQLite connection string from a potentially relative one.
    /// Ensures that relative paths are absolute based on the application's base directory.
    /// </summary>
    /// <param name="connectionString">The original connection string.</param>
    /// <returns>A connection string with an absolute path.</returns>
    public static string GetFullConnectionString(string connectionString)
    {
        var absolutePath = GetDatabaseFilePath(connectionString);
        return $"Data Source={absolutePath}";
    }
}
