namespace AdaptiveHypertrophy.Exercises;

public class IsolationExercise : Exercise
{
    public IsolationExercise(string name, string muscleGroup, string description = "")
        : this(name, muscleGroup, baseWeight: 0, targetReps: 12, accessoryFocus: string.Empty, description: description)
    {
    }

    public IsolationExercise(
        string name,
        string muscleGroup,
        double baseWeight,
        int targetReps,
        string accessoryFocus,
        string description = "")
        : base(name, muscleGroup, baseWeight, targetReps, description)
    {
        AccessoryFocus = accessoryFocus;
    }

    public string AccessoryFocus { get; }

    public override string GetDescription()
    {
        return string.IsNullOrEmpty(Description)
            ? $"{Name} - An isolation {MuscleGroup} exercise."
            : Description;
    }

    public override double EstimateVolume()
    {
        return BaseWeight * TargetReps;
    }

    public override string GetExerciseType()
    {
        return "Isolation";
    }
}
