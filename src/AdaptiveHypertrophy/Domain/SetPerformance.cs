namespace AdaptiveHypertrophy.Domain;

public readonly struct SetPerformance
{
    public SetPerformance(int reps, double weight, int exertion)
    {
        Reps = reps;
        Weight = weight;
        Exertion = exertion;
    }

    public int Reps { get; }

    public double Weight { get; }

    public int Exertion { get; }
}
