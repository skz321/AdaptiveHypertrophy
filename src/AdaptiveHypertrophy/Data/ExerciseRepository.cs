using AdaptiveHypertrophy.Exercises;
using Microsoft.Data.Sqlite;

namespace AdaptiveHypertrophy.Data
{
    public class ExerciseRepository : IRepository<Exercise>
    {
        private readonly DatabaseConnectionManager connectionManager;

        /// <summary>Uses the shared <see cref="DatabaseConnectionManager"/> singleton.</summary>
        public ExerciseRepository()
            : this(DatabaseConnectionManager.GetInstance())
        {
        }

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
                INSERT INTO Exercises (Name, IsCompound, MuscleGroup)
                VALUES (@Name, @IsCompound, @MuscleGroup);
            ";

            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@Name", entity.Name);
            command.Parameters.AddWithValue("@IsCompound", entity is CompoundExercise ? 1 : 0);
            command.Parameters.AddWithValue("@MuscleGroup", entity.MuscleGroup);

            command.ExecuteNonQuery();
        }

        public List<Exercise> GetAll()
        {
            List<Exercise> exercises = new List<Exercise>();

            using var connection = connectionManager.GetConnection();
            connection.Open();

            string sql = "SELECT Name, MuscleGroup, IsCompound FROM Exercises;";

            using var command = new SqliteCommand(sql, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                string name = reader.GetString(0);
                string muscleGroup = reader.GetString(1);
                bool isCompound = reader.GetInt32(2) != 0;

                if (isCompound)
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
                    Name TEXT NOT NULL UNIQUE,
                    IsCompound INTEGER NOT NULL,
                    MuscleGroup TEXT NOT NULL
                );
            ";

            using var command = new SqliteCommand(sql, connection);
            command.ExecuteNonQuery();
        }
    }
}