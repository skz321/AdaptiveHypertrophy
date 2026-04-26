using Microsoft.Data.Sqlite;

namespace AdaptiveHypertrophy.Data;

public sealed class DatabaseConnectionManager
{
    private static readonly Lazy<DatabaseConnectionManager> LazyInstance =
        new(() => new DatabaseConnectionManager());

    private readonly string _connectionString;

    private DatabaseConnectionManager()
    {
        _connectionString = "Data Source=adaptive_hypertrophy.db";
    }

    public static DatabaseConnectionManager GetInstance() => LazyInstance.Value;

    public void Connect()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
    }
}
