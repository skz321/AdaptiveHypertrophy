using AdaptiveHypertrophy.Domain;
using AdaptiveHypertrophy.Domain.Enums;
using AdaptiveHypertrophy.Exercises;

namespace AdaptiveHypertrophy.Planning;

public static class WorkoutPlanGenerator
{
    /// <summary>Builds week-one targets from stored profile max lifts (catalog keys only).</summary>
    public static WorkoutPlan BuildInitialPlan(User user, string? planName = null)
    {
        if (user is null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        if (user.MaxLiftsByKey.Count == 0)
        {
            throw new InvalidOperationException("Enter at least one max lift before generating a plan.");
        }

        string name = string.IsNullOrWhiteSpace(planName)
            ? $"Week 1 — {user.PreferredSplit} ({user.Goal})"
            : planName.Trim();

        double fraction = WorkingFractionOfMax(user.ExperienceLevel, user.Goal);
        int sets = Math.Clamp(user.WorkoutFrequency, 3, 5);

        var entries = new List<ExerciseEntry>();

        foreach (string key in ExerciseCatalog.OrderedKeys)
        {
            if (!user.MaxLiftsByKey.TryGetValue(key, out double max))
            {
                continue;
            }

            ExerciseCatalogEntry def = ExerciseCatalog.GetRequired(key);
            int targetReps = TargetRepsForGoal(user.Goal, def.IsCompound);
            double working = max * fraction * (def.IsCompound ? 1.0 : 0.88);

            Exercise ex = def.IsCompound
                ? new CompoundExercise(def.DisplayName, def.MuscleGroup, working, targetReps, mainLift: true)
                : new IsolationExercise(def.DisplayName, def.MuscleGroup, working, targetReps, def.AccessoryFocus);

            entries.Add(new ExerciseEntry(ex.Name, sets, targetReps, working, ex));
        }

        if (entries.Count == 0)
        {
            throw new InvalidOperationException(
                "No matching catalog lifts — use keys from the catalog list when entering maxes.");
        }

        return new WorkoutPlan(name, entries);
    }

    private static int TargetRepsForGoal(WorkoutGoal goal, bool isCompound)
    {
        return goal switch
        {
            WorkoutGoal.Strength => isCompound ? 5 : 8,
            WorkoutGoal.GeneralFitness => isCompound ? 8 : 10,
            _ => isCompound ? 10 : 12,
        };
    }

    /// <summary>Approximate first-session working intensity as a fraction of entered max (1RM or best set).</summary>
    private static double WorkingFractionOfMax(ExperienceLevel experience, WorkoutGoal goal)
    {
        double mid = experience switch
        {
            ExperienceLevel.Beginner => 0.68,
            ExperienceLevel.Intermediate => 0.75,
            ExperienceLevel.Advanced => 0.82,
            _ => 0.75,
        };

        return goal switch
        {
            WorkoutGoal.Strength => Math.Min(0.9, mid + 0.06),
            WorkoutGoal.GeneralFitness => Math.Max(0.6, mid - 0.03),
            _ => mid,
        };
    }
}
