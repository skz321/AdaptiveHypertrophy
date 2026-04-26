using AdaptiveHypertrophy.Domain.Enums;

namespace AdaptiveHypertrophy.Domain;

public sealed class User
{
    private static readonly Lazy<User> LazyInstance = new(() => new User());

    private User()
    {
    }

    public static User Instance => LazyInstance.Value;

    public int Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public int Age { get; private set; }

    public double BodyWeight { get; private set; }

    public string Gender { get; private set; } = string.Empty;

    public ExperienceLevel ExperienceLevel { get; private set; }

    public int WorkoutFrequency { get; private set; }

    public string PreferredSplit { get; private set; } = string.Empty;

    public WorkoutGoal Goal { get; private set; }

    public void UpdateProfile(
        int id,
        string name,
        int age,
        double bodyWeight,
        string gender,
        ExperienceLevel experienceLevel,
        int workoutFrequency,
        string preferredSplit,
        WorkoutGoal goal)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name is required.", nameof(name));
        }

        if (age < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(age), "Age cannot be negative.");
        }

        if (bodyWeight <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(bodyWeight), "Body weight must be positive.");
        }

        if (workoutFrequency < 1 || workoutFrequency > 7)
        {
            throw new ArgumentOutOfRangeException(nameof(workoutFrequency), "Workout frequency should be 1-7 days per week.");
        }

        Id = id;
        Name = name.Trim();
        Age = age;
        BodyWeight = bodyWeight;
        Gender = gender ?? string.Empty;
        ExperienceLevel = experienceLevel;
        WorkoutFrequency = workoutFrequency;
        PreferredSplit = preferredSplit?.Trim() ?? string.Empty;
        Goal = goal;
    }
}
