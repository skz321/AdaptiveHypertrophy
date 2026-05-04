using AdaptiveHypertrophy.Domain;

namespace AdaptiveHypertrophy.Exercises;

public abstract class Exercise
{
    protected Exercise(string name, string muscleGroup, double baseWeight, int targetReps, string description = "")
    {
        Name = name;
        MuscleGroup = muscleGroup;
        BaseWeight = baseWeight;
        TargetReps = targetReps;
        Description = description;
    }

    public string Name { get; }

    public string MuscleGroup { get; }

    public double BaseWeight { get; }

    public int TargetReps { get; }

    public string Description { get; }

    public string GetDisplayName() => Name;

    public abstract string GetDescription();

    public abstract double EstimateVolume();

    public abstract string GetExerciseType();
}
