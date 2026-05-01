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

    /// <summary>Opens the database once to verify connectivity and create the file if needed.</summary>
    public void Connect()
    {
        using var connection = GetConnection();
        connection.Open();
    }
}
