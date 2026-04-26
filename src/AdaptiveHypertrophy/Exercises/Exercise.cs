using AdaptiveHypertrophy.Domain;

namespace AdaptiveHypertrophy.Exercises;

public abstract class Exercise
{
    protected Exercise(string name, string muscleGroup, double baseWeight, int targetReps)
    {
        Name = name;
        MuscleGroup = muscleGroup;
        BaseWeight = baseWeight;
        TargetReps = targetReps;
    }

    public string Name { get; }

    public string MuscleGroup { get; }

    public double BaseWeight { get; }

    public int TargetReps { get; }

    public string GetDisplayName() => Name;

    public abstract double EstimateVolume();

    public abstract string GetExerciseType();
}
