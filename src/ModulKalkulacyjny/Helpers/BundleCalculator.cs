using ModulKalkulacyjny.Domain;

namespace ModulKalkulacyjny.Helpers;

/// <summary>
/// Oblicza zastępczy promień wiązki r_eq,k używany w wyrazie diagonalnym macierzy
/// współczynników potencjału elektrycznego:
///
///   P_kk = (1 / 2πε₀) · ln( 2·y_k / r_eq,k )
///
/// Każda wiązka jest zastępowana jednym równoważnym przewodem w centrum wiązki.
/// Parametr s_k to odległość między sąsiednimi podprzewodnikami.
/// </summary>
public static class BundleCalculator
{
    /// <summary>Zwraca liczbę podprzewodników b_k dla danego typu wiązki.</summary>
    public static int SubconductorCount(BundleType bundle) => bundle switch
    {
        BundleType.Single         => 1,
        BundleType.Twin           => 2,
        BundleType.TripleTriangle => 3,
        BundleType.QuadSquare     => 4,
        _ => throw new ArgumentOutOfRangeException(nameof(bundle), "Nieznany typ wiązki.")
    };

    /// <summary>
    /// Oblicza zastępczy promień wiązki r_eq,k [m].
    ///
    /// Wzory (r_s = promień podprzewodnika, s = rozstaw sąsiednich przewodów):
    ///
    ///   Single:           r_eq = r_s
    ///   Twin:             r_eq = √(r_s · s)
    ///   Triple triangle:  r_eq = (r_s · s²)^(1/3)
    ///   Quad square:      r_eq = (√2 · r_s · s³)^(1/4)
    ///
    /// Wynik jest przekazywany jako Conductor.Radius do PotentialMatrixBuilder.
    /// </summary>
    /// <param name="bundle">Typ geometryczny wiązki.</param>
    /// <param name="subRadius">Promień jednego podprzewodnika r_s [m]. Musi być dodatni.</param>
    /// <param name="spacing">
    ///   Odległość między sąsiednimi podprzewodnikami s_k [m].
    ///   Ignorowane dla Single; musi być dodatnie dla wszystkich innych typów.
    /// </param>
    public static double CalculateEquivalentRadius(
        BundleType bundle,
        double subRadius,
        double spacing = 0)
    {
        if (subRadius <= 0)
            throw new ArgumentException(
                $"Promień podprzewodnika musi być dodatni (podano {subRadius} m).", nameof(subRadius));

        if (bundle != BundleType.Single && spacing <= 0)
            throw new ArgumentException(
                $"Rozstaw wiązki musi być dodatni dla wiązki typu {bundle} (podano {spacing} m).", nameof(spacing));

        return bundle switch
        {
            // r_eq = r_s  (trivial – pojedynczy przewód)
            BundleType.Single =>
                subRadius,

            // r_eq = √(r_s · s)
            BundleType.Twin =>
                Math.Sqrt(subRadius * spacing),

            // r_eq = (r_s · s²)^(1/3)
            BundleType.TripleTriangle =>
                Math.Pow(subRadius * spacing * spacing, 1.0 / 3.0),

            // r_eq = (√2 · r_s · s³)^(1/4)
            BundleType.QuadSquare =>
                Math.Pow(Math.Sqrt(2.0) * subRadius * Math.Pow(spacing, 3), 0.25),

            _ => throw new ArgumentOutOfRangeException(nameof(bundle), "Nieznany typ wiązki.")
        };
    }
}

