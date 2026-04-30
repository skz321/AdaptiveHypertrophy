using AdaptiveHypertrophy.Domain;

namespace AdaptiveHypertrophy.Display
{
    public class ConsoleWorkoutDisplay : IWorkoutDisplay
    {
        public void ShowWorkoutLog(WorkoutLog workoutLog)
        {
            Console.WriteLine("Workout Log");
            Console.WriteLine("--------------------");
            Console.WriteLine($"Exercise: {workoutLog.ExerciseName}");
            Console.WriteLine($"Weight: {workoutLog.Weight} lbs");
            Console.WriteLine($"Reps: {workoutLog.Reps}");
            Console.WriteLine($"Sets: {workoutLog.Sets}");
            Console.WriteLine();
        }

        public void ShowWorkoutLogs(List<WorkoutLog> workoutLogs)
        {
            Console.WriteLine("Workout History");
            Console.WriteLine("--------------------");

            if (workoutLogs.Count == 0)
            {
                Console.WriteLine("No workout logs found.");
                return;
            }

            foreach (WorkoutLog workoutLog in workoutLogs)
            {
                ShowWorkoutLog(workoutLog);
            }
        }

        public string GetInput(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine() ?? "";
        }
    }
}