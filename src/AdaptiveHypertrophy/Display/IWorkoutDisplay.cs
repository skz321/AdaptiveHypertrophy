using AdaptiveHypertrophy.Domain;

namespace AdaptiveHypertrophy.Display;

public interface IWorkoutDisplay
{
    void ShowSetPerformance(SetPerformance performance);

    void ShowSetPerformances(IReadOnlyList<SetPerformance> performances);

    void ShowWorkoutPlan(WorkoutPlan plan);

    void ShowWorkoutLog(WorkoutLog workoutLog);

    void ShowWorkoutLogs(IReadOnlyList<WorkoutLog> workoutLogs);

    string GetInput(string prompt);
}
