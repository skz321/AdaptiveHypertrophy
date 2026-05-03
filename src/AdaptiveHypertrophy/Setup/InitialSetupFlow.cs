using AdaptiveHypertrophy.Display;
using AdaptiveHypertrophy.Domain;
using AdaptiveHypertrophy.Domain.Enums;
using AdaptiveHypertrophy.Planning;

namespace AdaptiveHypertrophy.Setup;

/// <summary>Console onboarding: profile, max lifts, and plan difficulty feedback (items 1–4 MVP).</summary>
public static class InitialSetupFlow
{
    private const double TooEasyFactor = 1.04;

    private const double TooHardFactor = 0.96;

    public static void RunProfilePrompts(IWorkoutDisplay display, User user)
    {
        Console.WriteLine("=== Profile setup ===");

        string name = PromptNonEmpty(display, "Display name: ");
        int age = ReadInt(display, "Age: ", min: 12, max: 90);
        double bw = ReadDouble(display, "Body weight (lbs): ", min: 60, max: 600);
        string gender = display.GetInput("Gender (free text): ").Trim();

        Console.WriteLine("Experience — 1 Beginner, 2 Intermediate, 3 Advanced");
        int expPick = ReadInt(display, "Pick (1-3): ", min: 1, max: 3);
        var experience = (ExperienceLevel)(expPick - 1);

        int freq = ReadInt(display, "Workouts per week (1-7): ", min: 1, max: 7);
        string split = PromptNonEmpty(display, "Preferred split (e.g. Upper/Lower, PPL, Full Body): ");

        Console.WriteLine("Goal — 1 Hypertrophy, 2 Strength, 3 General fitness");
        int goalPick = ReadInt(display, "Pick (1-3): ", min: 1, max: 3);
        var goal = (WorkoutGoal)(goalPick - 1);

        user.UpdateProfile(
            id: 1,
            name: name,
            age: age,
            bodyWeight: bw,
            gender: gender,
            experienceLevel: experience,
            workoutFrequency: freq,
            preferredSplit: split,
            goal: goal);

        Console.WriteLine();
    }

    public static void RunMaxLiftPrompts(IWorkoutDisplay display, User user)
    {
        Console.WriteLine("=== Max lifts (best recent max or estimated 1RM, lbs) ===");
        Console.WriteLine("Pick lifts from the catalog. Enter line as: <#> <weight>   Example: 1 185");
        Console.WriteLine("Enter 0 when finished.");
        Console.WriteLine();

        for (var i = 0; i < ExerciseCatalog.OrderedKeys.Count; i++)
        {
            string key = ExerciseCatalog.OrderedKeys[i];
            ExerciseCatalogEntry def = ExerciseCatalog.GetRequired(key);
            string tag = def.IsCompound ? "compound" : "isolation";
            Console.WriteLine($"  {i + 1}. {def.DisplayName} ({def.MuscleGroup}) — {tag}");
        }

        Console.WriteLine();
        user.ClearMaxLifts();

        while (true)
        {
            string line = display.GetInput("Lift # and max lbs (0 = done): ").Trim();
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 1)
            {
                continue;
            }

            if (!int.TryParse(parts[0], System.Globalization.NumberStyles.Integer,
                    System.Globalization.CultureInfo.InvariantCulture, out int pick))
            {
                Console.WriteLine("Start with the lift number, then weight.");
                continue;
            }

            if (pick == 0)
            {
                break;
            }

            if (pick < 1 || pick > ExerciseCatalog.OrderedKeys.Count)
            {
                Console.WriteLine($"Pick 1-{ExerciseCatalog.OrderedKeys.Count}.");
                continue;
            }

            if (parts.Length < 2 ||
                !double.TryParse(parts[1], System.Globalization.NumberStyles.Float,
                    System.Globalization.CultureInfo.InvariantCulture, out double max))
            {
                Console.WriteLine("Include max weight, e.g. \"1 185\".");
                continue;
            }

            string catalogKey = ExerciseCatalog.OrderedKeys[pick - 1];
            user.SetMaxLift(catalogKey, max);
            Console.WriteLine($"Saved {ExerciseCatalog.GetRequired(catalogKey).DisplayName}: {max} lbs.");
        }

        if (user.MaxLiftsByKey.Count == 0)
        {
            throw new InvalidOperationException("Enter at least one lift with a max before continuing.");
        }

        Console.WriteLine();
    }

    /// <summary>Too easy → raise weights; too hard → lower weights; accept → stop.</summary>
    public static WorkoutPlan RunPlanFeedbackLoop(IWorkoutDisplay display, WorkoutPlan initialPlan)
    {
        WorkoutPlan plan = initialPlan;

        while (true)
        {
            display.ShowWorkoutPlan(plan);
            Console.WriteLine("How does this starting plan look?");
            Console.WriteLine("  1 = Too easy (bump working weights ~4%)");
            Console.WriteLine("  2 = Too hard (drop working weights ~4%)");
            Console.WriteLine("  3 = Accept and continue");

            string raw = display.GetInput("Choice (1-3): ").Trim();
            switch (raw)
            {
                case "1":
                    plan = WorkoutPlanWeightScaler.ScaleWeights(plan, TooEasyFactor);
                    break;
                case "2":
                    plan = WorkoutPlanWeightScaler.ScaleWeights(plan, TooHardFactor);
                    break;
                case "3":
                    return plan;
                default:
                    Console.WriteLine("Enter 1, 2, or 3.");
                    break;
            }
        }
    }

    private static string PromptNonEmpty(IWorkoutDisplay display, string prompt)
    {
        while (true)
        {
            string s = display.GetInput(prompt).Trim();
            if (!string.IsNullOrWhiteSpace(s))
            {
                return s;
            }

            Console.WriteLine("Required.");
        }
    }

    private static double ReadDouble(IWorkoutDisplay display, string prompt, double min, double max)
    {
        while (true)
        {
            string raw = display.GetInput(prompt).Trim();
            if (double.TryParse(raw, System.Globalization.NumberStyles.Float,
                    System.Globalization.CultureInfo.InvariantCulture, out double value) &&
                value >= min && value <= max)
            {
                return value;
            }

            Console.WriteLine($"Enter a number between {min} and {max}.");
        }
    }

    private static int ReadInt(IWorkoutDisplay display, string prompt, int min, int max)
    {
        while (true)
        {
            string raw = display.GetInput(prompt).Trim();
            if (int.TryParse(raw, System.Globalization.NumberStyles.Integer,
                    System.Globalization.CultureInfo.InvariantCulture, out int value) &&
                value >= min && value <= max)
            {
                return value;
            }

            Console.WriteLine($"Enter an integer between {min} and {max}.");
        }
    }
}
