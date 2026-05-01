using AdaptiveHypertrophy.Domain;

namespace AdaptiveHypertrophy.Display;

public sealed class ConsoleWorkoutDisplay : IWorkoutDisplay
{
    public void ShowSetPerformance(SetPerformance performance)
    {
        Console.WriteLine(
            $"  Set — reps: {performance.Reps}, weight: {performance.Weight}, exertion (1–10): {performance.Exertion}");
    }

    public void ShowSetPerformances(IReadOnlyList<SetPerformance> performances)
    {
        Console.WriteLine("Set performances");
        Console.WriteLine(new string('-', 40));
        if (performances.Count == 0)
        {
            Console.WriteLine("  (none)");
            Console.WriteLine();
            return;
        }

        for (var i = 0; i < performances.Count; i++)
        {
            Console.Write($"{i + 1}. ");
            ShowSetPerformance(performances[i]);
        }

        Console.WriteLine();
    }

    public void ShowWorkoutPlan(WorkoutPlan plan)
    {
        Console.WriteLine($"Workout plan — {plan.Name}");
        Console.WriteLine(new string('-', 40));
        if (plan.Entries.Count == 0)
        {
            Console.WriteLine("  (no exercises)");
            Console.WriteLine();
            return;
        }

        Console.WriteLine(
            $"{TruncateName("Exercise"),-28} {"Sets",-6} {"Reps",-6} {"Wt (lbs)",-10}");
        Console.WriteLine(new string('-', 52));
        foreach (ExerciseEntry entry in plan.Entries)
        {
            Console.WriteLine(
                $"{TruncateName(entry.ExerciseName),-28} {entry.Sets,-6} {entry.Reps,-6} {entry.Weight,-10}");
        }

        Console.WriteLine();
    }

    public void ShowWorkoutLog(WorkoutLog workoutLog)
    {
        Console.WriteLine("Workout log");
        Console.WriteLine(new string('-', 40));
        if (workoutLog.Entries.Count == 0)
        {
            Console.WriteLine("  (no exercises logged)");
            Console.WriteLine();
            return;
        }

        Console.WriteLine(
            $"{TruncateName("Exercise"),-28} {"Sets",-6} {"Reps",-6} {"Wt",-10}");
        Console.WriteLine(new string('-', 52));
        foreach (ExerciseEntry entry in workoutLog.Entries)
        {
            Console.WriteLine(
                $"{TruncateName(entry.ExerciseName),-28} {entry.Sets,-6} {entry.Reps,-6} {entry.Weight,-10}");
        }

        Console.WriteLine();
    }

    public void ShowWorkoutLogs(IReadOnlyList<WorkoutLog> workoutLogs)
    {
        Console.WriteLine("Workout history");
        Console.WriteLine(new string('-', 40));
        if (workoutLogs.Count == 0)
        {
            Console.WriteLine("  No workouts recorded.");
            Console.WriteLine();
            return;
        }

        for (var i = 0; i < workoutLogs.Count; i++)
        {
            Console.WriteLine($"Session {i + 1}");
            ShowWorkoutLog(workoutLogs[i]);
        }
    }

    public string GetInput(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine() ?? "";
    }

    private static string TruncateName(string text) =>
        text.Length <= 26 ? text : text[..23] + "...";
}
