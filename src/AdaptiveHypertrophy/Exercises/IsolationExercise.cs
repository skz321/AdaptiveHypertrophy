namespace AdaptiveHypertrophy.Exercises;

public class IsolationExercise : Exercise
{
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
        throw new NotImplementedException();
    }

    public override string GetExerciseType()
    {
        throw new NotImplementedException();
    }
}
