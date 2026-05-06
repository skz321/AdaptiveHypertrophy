using Microsoft.Data.Sqlite;

namespace AdaptiveHypertrophy.Data;

/// <summary>
/// Thread-safe singleton: one shared handler for the SQLite file across the app.
/// Repositories obtain new <see cref="SqliteConnection"/> instances per operation (recommended for SQLite).
/// </summary>
public sealed class DatabaseConnectionManager
{
    private static readonly Lazy<DatabaseConnectionManager> LazyInstance =
        new(static () => new DatabaseConnectionManager());

    private readonly string _connectionString;

    private DatabaseConnectionManager()
    {
        string dbPath = Path.Combine(AppContext.BaseDirectory, "adaptive_hypertrophy.db");
        _connectionString = new SqliteConnectionStringBuilder
        {
            DataSource = dbPath,
            Mode = SqliteOpenMode.ReadWriteCreate,
        }.ToString();
    }

    /// <summary>Returns the single shared instance used by all repositories.</summary>
    public static DatabaseConnectionManager GetInstance() => LazyInstance.Value;

    /// <summary>Creates a new connection to the shared database (caller must dispose, typically via <c>using</c>).</summary>
    public SqliteConnection GetConnection() => new(_connectionString);

    /// <summary>Opens the database once to verify connectivity and creates tables if needed.</summary>
    public void Connect()
    {
        using var connection = GetConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS Exercises (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL UNIQUE,
                IsCompound INTEGER NOT NULL,
                MuscleGroup TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS Users (
                Id INTEGER PRIMARY KEY,
                Name TEXT NOT NULL,
                Age INTEGER NOT NULL,
                BodyWeight REAL NOT NULL,
                Gender TEXT NOT NULL,
                ExperienceLevel INTEGER NOT NULL,
                WorkoutFrequency INTEGER NOT NULL,
                PreferredSplit TEXT NOT NULL,
                Goal INTEGER NOT NULL
            );

            CREATE TABLE IF NOT EXISTS UserMaxLifts (
                ExerciseKey TEXT PRIMARY KEY,
                MaxWeight REAL NOT NULL
            );";
        command.ExecuteNonQuery();
    }
}
