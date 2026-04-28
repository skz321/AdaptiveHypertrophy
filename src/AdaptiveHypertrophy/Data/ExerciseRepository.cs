using AdaptiveHypertrophy.Exercises;
using Microsoft.Data.Sqlite;

namespace AdaptiveHypertrophy.Data
{
    public class ExerciseRepository : IRepository<Exercise>
    {
        private readonly DatabaseConnectionManager connectionManager;

        public ExerciseRepository(DatabaseConnectionManager connectionManager)
        {
            this.connectionManager = connectionManager;
            CreateTable();
        }

        public void Save(Exercise entity)
        {
            using var connection = connectionManager.GetConnection();
            connection.Open();

            string sql = @"
                INSERT INTO Exercises (Name, MuscleGroup, ExerciseType)
                VALUES (@Name, @MuscleGroup, @ExerciseType);
            ";

            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@Name", entity.Name);
            command.Parameters.AddWithValue("@MuscleGroup", entity.MuscleGroup.ToString());
            command.Parameters.AddWithValue("@ExerciseType", entity.GetType().Name);

            command.ExecuteNonQuery();
        }

        public List<Exercise> GetAll()
        {
            List<Exercise> exercises = new List<Exercise>();

            using var connection = connectionManager.GetConnection();
            connection.Open();

            string sql = "SELECT Name, MuscleGroup, ExerciseType FROM Exercises;";

            using var command = new SqliteCommand(sql, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                string name = reader.GetString(0);
                string muscleGroup = reader.GetString(1);
                string exerciseType = reader.GetString(2);

                if (exerciseType == "CompoundExercise")
                {
                    exercises.Add(new CompoundExercise(name, muscleGroup));
                }
                else
                {
                    exercises.Add(new IsolationExercise(name, muscleGroup));
                }
            }

            return exercises;
        }

        private void CreateTable()
        {
            using var connection = connectionManager.GetConnection();
            connection.Open();

            string sql = @"
                CREATE TABLE IF NOT EXISTS Exercises (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    MuscleGroup TEXT NOT NULL,
                    ExerciseType TEXT NOT NULL
                );
            ";

            using var command = new SqliteCommand(sql, connection);
            command.ExecuteNonQuery();
        }
    }
}