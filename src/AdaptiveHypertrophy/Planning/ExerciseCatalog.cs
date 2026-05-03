namespace AdaptiveHypertrophy.Planning;

/// <summary>Built-in lifts users can attach max weights to for initial plan generation.</summary>
public sealed record ExerciseCatalogEntry(
    string Key,
    string DisplayName,
    string MuscleGroup,
    bool IsCompound,
    string AccessoryFocus);

public static class ExerciseCatalog
{
    private static readonly Dictionary<string, ExerciseCatalogEntry> ByKey =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["bench"] = new ExerciseCatalogEntry(
                "bench", "Bench Press", "Chest", IsCompound: true, AccessoryFocus: ""),
            ["squat"] = new ExerciseCatalogEntry(
                "squat", "Squat", "Legs", IsCompound: true, AccessoryFocus: ""),
            ["deadlift"] = new ExerciseCatalogEntry(
                "deadlift", "Deadlift", "Back/Legs", IsCompound: true, AccessoryFocus: ""),
            ["ohp"] = new ExerciseCatalogEntry(
                "ohp", "Overhead Press", "Shoulders", IsCompound: true, AccessoryFocus: ""),
            ["row"] = new ExerciseCatalogEntry(
                "row", "Barbell Row", "Back", IsCompound: true, AccessoryFocus: ""),
            ["curl"] = new ExerciseCatalogEntry(
                "curl", "Barbell Curl", "Arms", IsCompound: false, AccessoryFocus: "biceps"),
        };

    /// <summary>Stable order for menus and plan listing.</summary>
    public static IReadOnlyList<string> OrderedKeys { get; } =
        new[] { "bench", "squat", "deadlift", "ohp", "row", "curl" };

    public static ExerciseCatalogEntry GetRequired(string key) =>
        ByKey.TryGetValue(key, out ExerciseCatalogEntry? e)
            ? e
            : throw new ArgumentException($"Unknown exercise key '{key}'.", nameof(key));
}
