using AdaptiveHypertrophy.Exercises;

namespace AdaptiveHypertrophy.Domain;

/// <summary>One exercise line on a plan (targets) or in a workout log (what was done).</summary>
public sealed record ExerciseEntry(
    string ExerciseName,
    int Sets,
    int Reps,
    double Weight,
    Exercise? LinkedExercise = null);
