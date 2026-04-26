using AdaptiveHypertrophy.Domain;
using AdaptiveHypertrophy.Exercises;

namespace AdaptiveHypertrophy.Progression;

public interface IProgressionStrategy
{
    double CalculateNextWeight(Exercise exercise, SetPerformance performance);

    int CalculateNextReps(Exercise exercise, SetPerformance performance);
}
