namespace AdaptiveHypertrophy.Domain;

public sealed class WorkoutSetLog
{
    public WorkoutSetLog(string exerciseName, DateTimeOffset performedAt, SetPerformance performance)
    {
        if (string.IsNullOrWhiteSpace(exerciseName))
        {
            throw new ArgumentException("Exercise name is required.", nameof(exerciseName));
        }

        ExerciseName = exerciseName.Trim();
        PerformedAt = performedAt;
        Performance = performance;
    }

    public string ExerciseName { get; }

    public DateTimeOffset PerformedAt { get; }

    public SetPerformance Performance { get; }
}

