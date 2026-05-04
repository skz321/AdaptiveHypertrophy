namespace AdaptiveHypertrophy.Exercises;

public class CompoundExercise : Exercise
{
    public CompoundExercise(string name, string muscleGroup, string description = "")
        : this(name, muscleGroup, baseWeight: 0, targetReps: 5, mainLift: true, description)
    {
    }

    public CompoundExercise(
        string name,
        string muscleGroup,
        double baseWeight,
        int targetReps,
        bool mainLift,
        string description = "")
        : base(name, muscleGroup, baseWeight, targetReps, description)
    {
        MainLift = mainLift;
    }

    public bool MainLift { get; }

    public override string GetDescription()
    {
        return string.IsNullOrEmpty(Description) 
            ? $"{Name} - A compound {MuscleGroup} exercise." 
            : Description;
    }

    public override double EstimateVolume()
    {
        return BaseWeight * TargetReps;
    }

    public override string GetExerciseType()
    {
        return MainLift ? "Main Compound" : "Compound";
    }
}
