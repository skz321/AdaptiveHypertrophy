using AdaptiveHypertrophy.Domain;
using AdaptiveHypertrophy.Exercises;

namespace AdaptiveHypertrophy.Progression;

public class HypertrophyProgressionStrategy : IProgressionStrategy
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

        var target = Math.Max(1, exercise.TargetReps);
        var topOfRange = target + 2;

        // Double progression (rep range first, then load):
        // - Work reps up within [target, target+2].
        // - Once at top of range with acceptable exertion, add a small load bump and reset reps to target.
        // - If exertion is extreme, slightly reduce load.
        double next =
            performance.Reps >= topOfRange && performance.Exertion <= 9
                ? baseWeight * 1.02
                : performance.Exertion >= 10
                    ? baseWeight * 0.97
                    : baseWeight;

        return RoundToIncrement(next, increment: 1.0);
    }

    public int CalculateNextReps(Exercise exercise, SetPerformance performance)
    {
        if (exercise is null)
        {
            throw new ArgumentNullException(nameof(exercise));
        }

        var target = Math.Max(1, exercise.TargetReps);
        var topOfRange = target + 2;

        if (performance.Exertion >= 10)
        {
            return Math.Max(1, target - 1);
        }

        if (performance.Reps >= topOfRange)
        {
            return target;
        }

        return Math.Min(topOfRange, Math.Max(target, performance.Reps + 1));
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
