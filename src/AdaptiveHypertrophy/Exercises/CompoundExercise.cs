namespace AdaptiveHypertrophy.Exercises;

public class CompoundExercise : Exercise
{
    public CompoundExercise(string name, string muscleGroup)
        : this(name, muscleGroup, baseWeight: 0, targetReps: 5, mainLift: true)
    {
    }

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
        return BaseWeight * TargetReps;
    }

    public override string GetExerciseType()
    {
        return MainLift ? "Main Compound" : "Compound";
    }
}
