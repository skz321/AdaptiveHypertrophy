using AdaptiveHypertrophy.Domain;
using AdaptiveHypertrophy.Domain.Enums;

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

Console.WriteLine($"{user.Name} | {user.ExperienceLevel} | {user.Goal}");
