namespace AdaptiveHypertrophy.Exercises;

public class CompoundExercise : Exercise
{
    public CompoundExercise(
        string name,
        string muscleGroup,
        double baseWeight,
        int targetReps,
        bool mainLift)
        : base(name, muscleGroup, baseWeight, targetReps)
    {
        MainLift = mainLift;
    }

    public bool MainLift { get; }

    public override double EstimateVolume()
    {
        throw new NotImplementedException();
    }

    public override string GetExerciseType()
    {
        throw new NotImplementedException();
    }
}
