using ModulKalkulacyjny.Domain;

namespace ModulKalkulacyjny.Helpers;

/// <summary>
/// Waliduje wszystkie dane wejściowe przed rozpoczęciem obliczeń.
/// Zgłasza <see cref="ArgumentException"/> z opisowym komunikatem przy pierwszym naruszeniu zasady.
/// </summary>
public static class InputValidator
{
    /// <summary>Sprawdza geometrię przewodów i wartości fazorów.</summary>
    public static void ValidateConductors(IReadOnlyList<Conductor> conductors)
    {
        if (conductors == null || conductors.Count == 0)
            throw new ArgumentException("Wymagany jest co najmniej jeden przewód.");

        foreach (var c in conductors)
        {
            if (!double.IsFinite(c.X) || !double.IsFinite(c.Y))
                throw new ArgumentException($"Przewód '{c.Id}': współrzędne muszą być skończonymi liczbami.");

            if (!double.IsFinite(c.Radius) || c.Radius <= 0)
                throw new ArgumentException($"Przewód '{c.Id}': promień musi być dodatnią liczbą skończoną.");

            if (c.Y <= c.Radius)
                throw new ArgumentException(
                    $"Przewód '{c.Id}': wysokość y ({c.Y} m) musi być większa niż promień ({c.Radius} m).");

            if (!double.IsFinite(c.Voltage.Real) || !double.IsFinite(c.Voltage.Imaginary))
                throw new ArgumentException($"Przewód '{c.Id}': fasor napięcia musi być skończony.");

            if (!double.IsFinite(c.Current.Real) || !double.IsFinite(c.Current.Imaginary))
                throw new ArgumentException($"Przewód '{c.Id}': fasor prądu musi być skończony.");
        }
    }

    /// <summary>
    /// Sprawdza, czy żaden punkt obserwacyjny nie leży wewnątrz ani dokładnie na powierzchni przewodu.
    /// </summary>
    public static void ValidateObservationPoints(
        IReadOnlyList<ObservationPoint> points,
        IReadOnlyList<Conductor> conductors)
    {
        foreach (var p in points)
        {
            if (!double.IsFinite(p.X) || !double.IsFinite(p.Y))
                throw new ArgumentException(
                    $"Punkt obserwacyjny ({p.X}, {p.Y}): współrzędne muszą być skończone.");

            foreach (var c in conductors)
            {
                double dist = Math.Sqrt(Math.Pow(p.X - c.X, 2) + Math.Pow(p.Y - c.Y, 2));

                if (dist < c.Radius)
                    throw new ArgumentException(
                        $"Punkt obserwacyjny ({p.X:F2}, {p.Y:F2}) leży wewnątrz przewodu '{c.Id}'.");

                if (dist < 1e-9)
                    throw new ArgumentException(
                        $"Punkt obserwacyjny ({p.X:F2}, {p.Y:F2}) jest zbyt blisko przewodu '{c.Id}'.");
            }
        }
    }

    /// <summary>
    /// Sprawdza, czy każdy stan pracy ma poprawną liczbę fazorów i dodatni czas trwania.
    /// </summary>
    public static void ValidateOperatingStates(
        IReadOnlyList<OperatingState> states,
        int conductorCount)
    {
        if (states == null || states.Count == 0)
            throw new ArgumentException("Wymagany jest co najmniej jeden stan pracy.");

        for (int i = 0; i < states.Count; i++)
        {
            var s = states[i];

            if (s.DurationSeconds <= 0)
                throw new ArgumentException($"Stan pracy {i + 1}: czas trwania musi być dodatni.");

            if (s.Voltages.Count != conductorCount)
                throw new ArgumentException(
                    $"Stan pracy {i + 1}: liczba napięć ({s.Voltages.Count}) " +
                    $"musi być równa liczbie przewodów ({conductorCount}).");

            if (s.Currents.Count != conductorCount)
                throw new ArgumentException(
                    $"Stan pracy {i + 1}: liczba prądów ({s.Currents.Count}) " +
                    $"musi być równa liczbie przewodów ({conductorCount}).");
        }
    }

    /// <summary>Sprawdza, czy wykładnik ekspozycji p jest dodatni.</summary>
    public static void ValidateExponent(double p)
    {
        if (p <= 0)
            throw new ArgumentException("Wykładnik ekspozycji p musi być dodatni.");
    }
}

