using AdaptiveHypertrophy.Domain;

namespace AdaptiveHypertrophy.Display;

public interface IWorkoutDisplay
{
    void ShowSetPerformance(SetPerformance performance);

    void ShowSetPerformances(List<SetPerformance> performances);

    string GetInput(string prompt);
}