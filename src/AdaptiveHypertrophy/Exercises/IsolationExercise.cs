namespace AdaptiveHypertrophy.Exercises;

public class IsolationExercise : Exercise
{
    public IsolationExercise(string name, string muscleGroup)
        : this(name, muscleGroup, baseWeight: 0, targetReps: 12, accessoryFocus: string.Empty)
    {
    }

    public IsolationExercise(
        string name,
        string muscleGroup,
        double baseWeight,
        int targetReps,
        string accessoryFocus)
        : base(name, muscleGroup, baseWeight, targetReps)
    {
        AccessoryFocus = accessoryFocus;
    }

    public string AccessoryFocus { get; }

    public override double EstimateVolume()
    {
        return BaseWeight * TargetReps;
    }

    public override string GetExerciseType()
    {
        return "Isolation";
    }
}
