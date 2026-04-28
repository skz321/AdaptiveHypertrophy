namespace AdaptiveHypertrophy.Domain
{
    public class WorkoutLog
    {
        public string ExerciseName { get; set; }
        public double Weight { get; set; }
        public int Reps { get; set; }
        public int Sets { get; set; }

        public WorkoutLog(string exerciseName, double weight, int reps, int sets)
        {
            ExerciseName = exerciseName;
            Weight = weight;
            Reps = reps;
            Sets = sets;
        }
    }
}