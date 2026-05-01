using AdaptiveHypertrophy.Domain;
using Microsoft.Data.Sqlite;

namespace AdaptiveHypertrophy.Data
{
    public class WorkoutRepository : IRepository<WorkoutLog>
    {
        private readonly DatabaseConnectionManager connectionManager;

        /// <summary>Uses the shared <see cref="DatabaseConnectionManager"/> singleton.</summary>
        public WorkoutRepository()
            : this(DatabaseConnectionManager.GetInstance())
        {
        }

        public WorkoutRepository(DatabaseConnectionManager connectionManager)
        {
            this.connectionManager = connectionManager;
            CreateTable();
        }

        public void Save(WorkoutLog entity)
        {
            using var connection = connectionManager.GetConnection();
            connection.Open();

            const string sql = @"
                INSERT INTO WorkoutLogs (ExerciseName, Weight, Reps, Sets)
                VALUES (@ExerciseName, @Weight, @Reps, @Sets);
            ";

            foreach (ExerciseEntry entry in entity.Entries)
            {
                using var command = new SqliteCommand(sql, connection);
                command.Parameters.AddWithValue("@ExerciseName", entry.ExerciseName);
                command.Parameters.AddWithValue("@Weight", entry.Weight);
                command.Parameters.AddWithValue("@Reps", entry.Reps);
                command.Parameters.AddWithValue("@Sets", entry.Sets);

                command.ExecuteNonQuery();
            }
        }

        public List<WorkoutLog> GetAll()
        {
            var workoutLogs = new List<WorkoutLog>();

            using var connection = connectionManager.GetConnection();
            connection.Open();

            const string sql = "SELECT ExerciseName, Weight, Reps, Sets FROM WorkoutLogs ORDER BY Id;";

            using var command = new SqliteCommand(sql, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                string exerciseName = reader.GetString(0);
                double weight = reader.GetDouble(1);
                int reps = reader.GetInt32(2);
                int sets = reader.GetInt32(3);

                workoutLogs.Add(new WorkoutLog(exerciseName, weight, reps, sets));
            }

            return workoutLogs;
        }

        private void CreateTable()
        {
            using var connection = connectionManager.GetConnection();
            connection.Open();

            const string sql = @"
                CREATE TABLE IF NOT EXISTS WorkoutLogs (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ExerciseName TEXT NOT NULL,
                    Weight REAL NOT NULL,
                    Reps INTEGER NOT NULL,
                    Sets INTEGER NOT NULL
                );
            ";

            using var command = new SqliteCommand(sql, connection);
            command.ExecuteNonQuery();
        }
    }
}
