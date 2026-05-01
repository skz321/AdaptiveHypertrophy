using AdaptiveHypertrophy.Domain;
using AdaptiveHypertrophy.Exercises;

namespace AdaptiveHypertrophy.Progression;

public class StrengthProgressionStrategy : IProgressionStrategy
{
    public double CalculateNextWeight(Exercise exercise, SetPerformance performance)
    {
        if (exercise is null)
        {
            throw new ArgumentNullException(nameof(exercise));
        }

        var baseWeight = performance.Weight > 0 ? performance.Weight : exercise.BaseWeight;
        if (baseWeight <= 0)
        {
            return 0;
        }

        var isMainLift = exercise is CompoundExercise { MainLift: true };
        var target = Math.Max(1, exercise.TargetReps);

        // Simple "keep/add/deload" strength rule based on reps + exertion (RPE-ish 1-10).
        // - Hit target with reasonable exertion: add weight.
        // - Miss target badly or exertion very high: small deload.
        // - Otherwise: keep weight.
        var increasePct = isMainLift ? 0.025 : 0.0125;
        var deloadPct = isMainLift ? 0.025 : 0.015;

        double next =
            performance.Reps >= target && performance.Exertion <= 8
                ? baseWeight * (1 + increasePct)
                : performance.Exertion >= 10 || performance.Reps < Math.Max(1, target - 2)
                    ? baseWeight * (1 - deloadPct)
                    : baseWeight;

        return RoundToIncrement(next, isMainLift ? 2.5 : 1.0);
    }

    public int CalculateNextReps(Exercise exercise, SetPerformance performance)
    {
        if (exercise is null)
        {
            throw new ArgumentNullException(nameof(exercise));
        }

        var target = Math.Max(1, exercise.TargetReps);

        if (performance.Exertion >= 10)
        {
            return Math.Max(1, target - 1);
        }

        // Strength work is usually fixed-rep: keep the target unless the set was very easy.
        if (performance.Reps >= target && performance.Exertion <= 6)
        {
            return target + 1;
        }

        return target;
    }

    private static double RoundToIncrement(double weight, double increment)
    {
        if (increment <= 0)
        {
            return weight;
        }

        return Math.Round(weight / increment, MidpointRounding.AwayFromZero) * increment;
    }
}
