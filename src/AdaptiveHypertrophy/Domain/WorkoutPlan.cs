using AdaptiveHypertrophy.Exercises;
using AdaptiveHypertrophy.Progression;

namespace AdaptiveHypertrophy.Domain;

public sealed class WorkoutPlan
{
    public WorkoutPlan(string name, List<ExerciseEntry> entries)
    {
        Name = string.IsNullOrWhiteSpace(name) ? "Plan" : name.Trim();
        Entries = entries ?? throw new ArgumentNullException(nameof(entries));
    }

    public string Name { get; }

    public List<ExerciseEntry> Entries { get; }

    public WorkoutPlan ApplyProgression(
        IReadOnlyDictionary<string, SetPerformance> lastSetByExerciseName,
        IProgressionStrategy progression)
    {
        if (lastSetByExerciseName is null)
        {
            throw new ArgumentNullException(nameof(lastSetByExerciseName));
        }

        if (progression is null)
        {
            throw new ArgumentNullException(nameof(progression));
        }

        List<ExerciseEntry> nextEntries = Entries
            .Select(e =>
            {
                Exercise? linked = e.LinkedExercise;
                string lookupName = linked?.Name ?? e.ExerciseName;
                if (linked is null || !lastSetByExerciseName.TryGetValue(lookupName, out SetPerformance last))
                {
                    return e;
                }

                Exercise ex = linked;
                double nextWeight = progression.CalculateNextWeight(ex, last);
                int nextReps = progression.CalculateNextReps(ex, last);
                return e with { Weight = nextWeight, Reps = nextReps };
            })
            .ToList();

        return new WorkoutPlan(Name, nextEntries);
    }
}
