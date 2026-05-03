using AdaptiveHypertrophy.Data;
using AdaptiveHypertrophy.Display;
using AdaptiveHypertrophy.Domain;
using AdaptiveHypertrophy.Domain.Enums;
using AdaptiveHypertrophy.Exercises;
using AdaptiveHypertrophy.Planning;
using AdaptiveHypertrophy.Progression;
using AdaptiveHypertrophy.Setup;

var user = User.Instance;

IWorkoutDisplay display = new ConsoleWorkoutDisplay();

DatabaseConnectionManager.GetInstance().Connect();

var exerciseRepo = new ExerciseRepository();
var workoutRepo = new WorkoutRepository();

InitialSetupFlow.RunProfilePrompts(display, user);
InitialSetupFlow.RunMaxLiftPrompts(display, user);

WorkoutPlan draftPlan = WorkoutPlanGenerator.BuildInitialPlan(user);
WorkoutPlan acceptedPlan = InitialSetupFlow.RunPlanFeedbackLoop(display, draftPlan);

Console.WriteLine("Final plan saved for this session.");
Console.WriteLine($"{user.Name} | {user.ExperienceLevel} | {user.Goal} | {user.WorkoutFrequency}x/wk | {user.PreferredSplit}");
Console.WriteLine();

HashSet<string> savedNames = exerciseRepo.GetAll()
    .Select(e => e.Name)
    .ToHashSet(StringComparer.OrdinalIgnoreCase);

foreach (ExerciseEntry entry in acceptedPlan.Entries)
{
    if (entry.LinkedExercise is null)
    {
        continue;
    }

    string n = entry.LinkedExercise.Name;
    if (savedNames.Contains(n))
    {
        continue;
    }

    exerciseRepo.Save(entry.LinkedExercise);
    savedNames.Add(n);
}

// Demo: progression preview using sample performance on the first lifts in the plan.
var lastLift = new Dictionary<string, SetPerformance>(StringComparer.OrdinalIgnoreCase);
foreach (ExerciseEntry entry in acceptedPlan.Entries.Take(2))
{
    if (entry.LinkedExercise is null)
    {
        continue;
    }

    Exercise ex = entry.LinkedExercise;
    lastLift[ex.Name] = new SetPerformance(
        reps: entry.Reps + 1,
        weight: entry.Weight,
        exertion: 7);
}

if (lastLift.Count > 0)
{
    display.ShowSetPerformances(lastLift.Values.ToList());

    IProgressionStrategy progression = user.Goal switch
    {
        WorkoutGoal.Strength => new StrengthProgressionStrategy(),
        _ => new HypertrophyProgressionStrategy(),
    };

    WorkoutPlan nextPlan = acceptedPlan.ApplyProgression(lastLift, progression);

    Console.WriteLine("Sample next-session targets (after adaptive progression from demo performance):");
    display.ShowWorkoutPlan(nextPlan);
}

Console.WriteLine($"SQLite: {exerciseRepo.GetAll().Count} exercise(s) in catalog, {workoutRepo.GetAll().Count} workout log row(s).");
Console.WriteLine();

Console.WriteLine("Optional — log a workout (enter numbers; blank exercise name to finish):");

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
