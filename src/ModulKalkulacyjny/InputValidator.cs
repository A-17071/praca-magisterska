namespace ModulKalkulacyjny;

/// <summary>
/// Validates all input data before starting a calculation.
/// Throws <see cref="ArgumentException"/> with a descriptive message on the first violated rule.
/// </summary>
public static class InputValidator
{
    /// <summary>Checks conductor geometry and phasor values.</summary>
    public static void ValidateConductors(IReadOnlyList<Conductor> conductors)
    {
        if (conductors == null || conductors.Count == 0)
            throw new ArgumentException("At least one conductor is required.");

        foreach (var c in conductors)
        {
            if (!double.IsFinite(c.X) || !double.IsFinite(c.Y))
                throw new ArgumentException($"Conductor '{c.Id}': coordinates must be finite numbers.");

            if (!double.IsFinite(c.Radius) || c.Radius <= 0)
                throw new ArgumentException($"Conductor '{c.Id}': radius must be a positive finite number.");

            if (c.Y <= c.Radius)
                throw new ArgumentException(
                    $"Conductor '{c.Id}': height y ({c.Y} m) must be greater than radius ({c.Radius} m).");

            if (!double.IsFinite(c.Voltage.Real) || !double.IsFinite(c.Voltage.Imaginary))
                throw new ArgumentException($"Conductor '{c.Id}': voltage phasor must be finite.");

            if (!double.IsFinite(c.Current.Real) || !double.IsFinite(c.Current.Imaginary))
                throw new ArgumentException($"Conductor '{c.Id}': current phasor must be finite.");
        }
    }

    /// <summary>
    /// Checks that no observation point lies inside or exactly on a conductor.
    /// </summary>
    public static void ValidateObservationPoints(
        IReadOnlyList<ObservationPoint> points,
        IReadOnlyList<Conductor> conductors)
    {
        foreach (var p in points)
        {
            if (!double.IsFinite(p.X) || !double.IsFinite(p.Y))
                throw new ArgumentException(
                    $"Observation point ({p.X}, {p.Y}): coordinates must be finite.");

            foreach (var c in conductors)
            {
                double dist = Math.Sqrt(Math.Pow(p.X - c.X, 2) + Math.Pow(p.Y - c.Y, 2));

                if (dist < c.Radius)
                    throw new ArgumentException(
                        $"Observation point ({p.X:F2}, {p.Y:F2}) lies inside conductor '{c.Id}'.");

                if (dist < 1e-9)
                    throw new ArgumentException(
                        $"Observation point ({p.X:F2}, {p.Y:F2}) is too close to conductor '{c.Id}'.");
            }
        }
    }

    /// <summary>
    /// Checks that each operating state has the correct number of phasors and a positive duration.
    /// </summary>
    public static void ValidateOperatingStates(
        IReadOnlyList<OperatingState> states,
        int conductorCount)
    {
        if (states == null || states.Count == 0)
            throw new ArgumentException("At least one operating state is required.");

        for (int i = 0; i < states.Count; i++)
        {
            var s = states[i];

            if (s.DurationSeconds <= 0)
                throw new ArgumentException($"Operating state {i + 1}: duration must be positive.");

            if (s.Voltages.Count != conductorCount)
                throw new ArgumentException(
                    $"Operating state {i + 1}: voltage count ({s.Voltages.Count}) " +
                    $"must match conductor count ({conductorCount}).");

            if (s.Currents.Count != conductorCount)
                throw new ArgumentException(
                    $"Operating state {i + 1}: current count ({s.Currents.Count}) " +
                    $"must match conductor count ({conductorCount}).");
        }
    }

    /// <summary>Checks that the exposure exponent p is positive.</summary>
    public static void ValidateExponent(double p)
    {
        if (p <= 0)
            throw new ArgumentException("Exposure exponent p must be positive.");
    }
}

