namespace AdaptiveHypertrophy.Domain
{
    public class WorkoutLog
    {
        public WorkoutLog()
        {
            Entries = new List<ExerciseEntry>();
        }

        public WorkoutLog(IEnumerable<ExerciseEntry> entries)
        {
            Entries = new List<ExerciseEntry>(entries);
        }

        /// <summary>Single-log row compatibility (repository / migrations).</summary>
        public WorkoutLog(string exerciseName, double weight, int reps, int sets)
            : this(new[] { new ExerciseEntry(exerciseName, sets, reps, weight) })
        {
        }

        public List<ExerciseEntry> Entries { get; }
    }
}
