using AdaptiveHypertrophy.Data;
using AdaptiveHypertrophy.Display;
using AdaptiveHypertrophy.Domain;
using AdaptiveHypertrophy.Domain.Enums;
using AdaptiveHypertrophy.Exercises;
using AdaptiveHypertrophy.Progression;

var user = User.Instance;
user.UpdateProfile(
    id: 1,
    name: "Demo User",
    age: 25,
    bodyWeight: 80,
    gender: "Unspecified",
    experienceLevel: ExperienceLevel.Intermediate,
    workoutFrequency: 4,
    preferredSplit: "Upper/Lower",
    goal: WorkoutGoal.Hypertrophy);

IWorkoutDisplay display = new ConsoleWorkoutDisplay();

DatabaseConnectionManager.GetInstance().Connect();

var exerciseRepo = new ExerciseRepository();
var workoutRepo = new WorkoutRepository();

Console.WriteLine($"{user.Name} | {user.ExperienceLevel} | {user.Goal}");
Console.WriteLine();

var bench = new CompoundExercise("Bench Press", "Chest", baseWeight: 185, targetReps: 5, mainLift: true);
var curls = new IsolationExercise("Barbell Curl", "Arms", baseWeight: 50, targetReps: 12, accessoryFocus: "biceps");

var plan = new WorkoutPlan(
    "Upper — strength bias",
    new List<ExerciseEntry>
    {
        new(bench.Name, Sets: 3, bench.TargetReps, bench.BaseWeight, bench),
        new(curls.Name, Sets: 3, curls.TargetReps, curls.BaseWeight, curls),
    });

display.ShowWorkoutPlan(plan);

var lastLift = new Dictionary<string, SetPerformance>(StringComparer.OrdinalIgnoreCase)
{
    [bench.Name] = new SetPerformance(reps: 6, weight: 185, exertion: 7),
    [curls.Name] = new SetPerformance(reps: 13, weight: 50, exertion: 7),
};

var performances = lastLift.Values.ToList();

Console.WriteLine("Last session performance (fed into progression):");
display.ShowSetPerformances(performances);

IProgressionStrategy progression =
    user.Goal == WorkoutGoal.Hypertrophy
        ? new HypertrophyProgressionStrategy()
        : new StrengthProgressionStrategy();

WorkoutPlan nextPlan = plan.ApplyProgression(lastLift, progression);

Console.WriteLine("Next session targets (after adaptive progression):");
display.ShowWorkoutPlan(nextPlan);

if (exerciseRepo.GetAll().Count == 0)
{
    exerciseRepo.Save(bench);
    exerciseRepo.Save(curls);
}

Console.WriteLine($"SQLite: {exerciseRepo.GetAll().Count} exercise(s) in catalog, {workoutRepo.GetAll().Count} workout log row(s).");
Console.WriteLine();

Console.WriteLine("Build a quick log from console (enter numbers; blank name to finish):");

WorkoutLog createdLog = new WorkoutLog();
while (true)
{
    string name = display.GetInput("Exercise name: ").Trim();
    if (string.IsNullOrWhiteSpace(name))
    {
        break;
    }

    if (!TryReadInt(display, "Sets: ", out int sets) ||
        !TryReadInt(display, "Reps: ", out int reps) ||
        !TryReadDouble(display, "Weight: ", out double weight))
    {
        Console.WriteLine("Invalid number; try again or leave name blank to finish.");
        continue;
    }

    createdLog.Entries.Add(new ExerciseEntry(name, sets, reps, weight));
}

if (createdLog.Entries.Count > 0)
{
    display.ShowWorkoutLog(createdLog);

    Console.WriteLine("Saving log rows to SQLite (if configured)…");
    try
    {
        workoutRepo.Save(createdLog);

        Console.WriteLine("Stored. Recent rows from DB (one row → one workout log bucket):");
        display.ShowWorkoutLogs(workoutRepo.GetAll());
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database skipped — {ex.Message}");
    }
}
else if (TrySampleDb(display, workoutRepo))
{
}

static bool TryReadDouble(IWorkoutDisplay d, string prompt, out double value)
{
    string raw = d.GetInput(prompt).Trim();
    return double.TryParse(raw, System.Globalization.NumberStyles.Float,
        System.Globalization.CultureInfo.InvariantCulture, out value);
}

static bool TryReadInt(IWorkoutDisplay d, string prompt, out int value)
{
    string raw = d.GetInput(prompt).Trim();
    return int.TryParse(raw, System.Globalization.NumberStyles.Integer,
        System.Globalization.CultureInfo.InvariantCulture, out value);
}

/// <summary>Non-interactive path so CI / quick run shows DB + displays without typing.</summary>
static bool TrySampleDb(IWorkoutDisplay display, WorkoutRepository workoutRepo)
{
    try
    {
        var sample = new WorkoutLog(
            new[]
            {
                new ExerciseEntry("Pull-up", Sets: 3, Reps: 8, Weight: 0),
                new ExerciseEntry("Leg Press", Sets: 4, Reps: 12, Weight: 270),
            });
        workoutRepo.Save(sample);
        Console.WriteLine("Sample multi-exercise log saved; reading back:");
        display.ShowWorkoutLogs(workoutRepo.GetAll());
        return true;
    }
    catch
    {
        return false;
    }
}
