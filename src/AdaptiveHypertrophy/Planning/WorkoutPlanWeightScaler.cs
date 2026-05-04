using AdaptiveHypertrophy.Domain;
using AdaptiveHypertrophy.Exercises;

namespace AdaptiveHypertrophy.Planning;

/// <summary>Adjusts displayed working weights (and linked exercise baselines) for the setup feedback loop.</summary>
public static class WorkoutPlanWeightScaler
{
    public static WorkoutPlan ScaleWeights(WorkoutPlan plan, double factor)
    {
        if (plan is null)
        {
            throw new ArgumentNullException(nameof(plan));
        }

        if (factor <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(factor), "Scale factor must be positive.");
        }

        List<ExerciseEntry> next = plan.Entries
            .Select(e =>
            {
                double w = RoundWorkingWeight(e.Weight * factor);
                Exercise? linked = e.LinkedExercise is null
                    ? null
                    : CloneExerciseScaled(e.LinkedExercise, factor);
                return e with { Weight = w, LinkedExercise = linked };
            })
            .ToList();

        return new WorkoutPlan(plan.Name, next);
    }

    private static Exercise CloneExerciseScaled(Exercise ex, double factor)
    {
        return ex switch
        {
            CompoundExercise c => new CompoundExercise(
                c.Name,
                c.MuscleGroup,
                RoundWorkingWeight(c.BaseWeight * factor),
                c.TargetReps,
                c.MainLift,
                c.Description),
            IsolationExercise i => new IsolationExercise(
                i.Name,
                i.MuscleGroup,
                RoundWorkingWeight(i.BaseWeight * factor),
                i.TargetReps,
                i.AccessoryFocus,
                i.Description),
            _ => throw new InvalidOperationException($"Unsupported exercise type {ex.GetType().Name}."),
        };
    }

    private static double RoundWorkingWeight(double weight)
    {
        if (weight <= 0)
        {
            return 0;
        }

        // Friendly gym increments (half-pound minimum step for light loads).
        double increment = weight >= 45 ? 2.5 : 1.0;
        return Math.Round(weight / increment, MidpointRounding.AwayFromZero) * increment;
    }
}
