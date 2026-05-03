using AdaptiveHypertrophy.Domain;
using AdaptiveHypertrophy.Domain.Enums;
using Microsoft.Data.Sqlite;
using System.Globalization;

namespace AdaptiveHypertrophy.Data;

public class UserRepository
{
    private readonly DatabaseConnectionManager _manager;

    public UserRepository()
    {
        _manager = DatabaseConnectionManager.GetInstance();
    }

    public void SaveUser(User user)
    {
        using var connection = _manager.GetConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        // Save Profile
        using var cmd = connection.CreateCommand();
        cmd.Transaction = transaction;
        cmd.CommandText = @"
            INSERT INTO Users (Id, Name, Age, BodyWeight, Gender, ExperienceLevel, WorkoutFrequency, PreferredSplit, Goal)
            VALUES (@id, @name, @age, @weight, @gender, @experience, @freq, @split, @goal)
            ON CONFLICT(Id) DO UPDATE SET
                Name=excluded.Name,
                Age=excluded.Age,
                BodyWeight=excluded.BodyWeight,
                Gender=excluded.Gender,
                ExperienceLevel=excluded.ExperienceLevel,
                WorkoutFrequency=excluded.WorkoutFrequency,
                PreferredSplit=excluded.PreferredSplit,
                Goal=excluded.Goal;";
                
        cmd.Parameters.AddWithValue("@id", user.Id == 0 ? 1 : user.Id);
        cmd.Parameters.AddWithValue("@name", user.Name ?? "");
        cmd.Parameters.AddWithValue("@age", user.Age);
        cmd.Parameters.AddWithValue("@weight", user.BodyWeight);
        cmd.Parameters.AddWithValue("@gender", user.Gender ?? "");
        cmd.Parameters.AddWithValue("@experience", (int)user.ExperienceLevel);
        cmd.Parameters.AddWithValue("@freq", user.WorkoutFrequency);
        cmd.Parameters.AddWithValue("@split", user.PreferredSplit ?? "");
        cmd.Parameters.AddWithValue("@goal", (int)user.Goal);
        cmd.ExecuteNonQuery();

        // Save Max Lifts
        using var clearLiftsCmd = connection.CreateCommand();
        clearLiftsCmd.Transaction = transaction;
        clearLiftsCmd.CommandText = "DELETE FROM UserMaxLifts;";
        clearLiftsCmd.ExecuteNonQuery();

        using var insertLiftCmd = connection.CreateCommand();
        insertLiftCmd.Transaction = transaction;
        insertLiftCmd.CommandText = "INSERT INTO UserMaxLifts (ExerciseKey, MaxWeight) VALUES (@key, @weight);";
        
        var keyParam = insertLiftCmd.Parameters.Add("@key", SqliteType.Text);
        var weightParam = insertLiftCmd.Parameters.Add("@weight", SqliteType.Real);

        foreach (var kvp in user.MaxLiftsByKey)
        {
            keyParam.Value = kvp.Key;
            weightParam.Value = kvp.Value;
            insertLiftCmd.ExecuteNonQuery();
        }

        transaction.Commit();
    }

    public bool LoadUser(User user)
    {
        using var connection = _manager.GetConnection();
        connection.Open();

        using var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT Id, Name, Age, BodyWeight, Gender, ExperienceLevel, WorkoutFrequency, PreferredSplit, Goal FROM Users LIMIT 1;";
        using var reader = cmd.ExecuteReader();

        if (!reader.Read())
            return false;

        int id = reader.GetInt32(0);
        string name = reader.GetString(1);
        int age = reader.GetInt32(2);
        double weight = reader.GetDouble(3);
        string gender = reader.GetString(4);
        ExperienceLevel experience = (ExperienceLevel)reader.GetInt32(5);
        int freq = reader.GetInt32(6);
        string split = reader.GetString(7);
        WorkoutGoal goal = (WorkoutGoal)reader.GetInt32(8);

        user.UpdateProfile(id, name, age, weight, gender, experience, freq, split, goal);

        // Load Max Lifts
        user.ClearMaxLifts();
        using var liftsCmd = connection.CreateCommand();
        liftsCmd.CommandText = "SELECT ExerciseKey, MaxWeight FROM UserMaxLifts;";
        using var liftsReader = liftsCmd.ExecuteReader();
        while (liftsReader.Read())
        {
            user.SetMaxLift(liftsReader.GetString(0), liftsReader.GetDouble(1));
        }

        return true;
    }

    public void ClearAllUserData()
    {
        using var connection = _manager.GetConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        using var cmd1 = connection.CreateCommand();
        cmd1.Transaction = transaction;
        cmd1.CommandText = "DELETE FROM Users;";
        cmd1.ExecuteNonQuery();

        using var cmd2 = connection.CreateCommand();
        cmd2.Transaction = transaction;
        cmd2.CommandText = "DELETE FROM UserMaxLifts;";
        cmd2.ExecuteNonQuery();

        // Optional: clear workout logs too if you want a complete reset
        using var cmd3 = connection.CreateCommand();
        cmd3.Transaction = transaction;
        cmd3.CommandText = "DELETE FROM WorkoutLogs;";
        cmd3.ExecuteNonQuery();

        transaction.Commit();
    }
}
