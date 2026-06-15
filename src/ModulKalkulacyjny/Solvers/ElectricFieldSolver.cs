using System.Numerics;
using ModulKalkulacyjny.Domain;
using ModulKalkulacyjny.Helpers;

namespace ModulKalkulacyjny.Solvers;

/// <summary>
/// Wyznacza fazory liniowej gęstości ładunku i oblicza pole elektryczne.
///
/// Proces dwuetapowy:
///   1. Rozwiąż  P · q = U  →  liniowe gęstości ładunku q
///   2. Dla każdego punktu obserwacyjnego: E = −∇V  (ujemny gradient potencjału)
/// </summary>
public sealed class ElectricFieldSolver
{
    private readonly LinearSystemSolver _linSolver = new();

    /// <summary>
    /// Rozwiązuje układ liniowy P · q = U w celu wyznaczenia fazorów liniowej gęstości ładunku [C/m].
    /// </summary>
    public Complex[] SolveLineCharges(double[,] potentialMatrix, Complex[] voltages)
        => _linSolver.Solve(potentialMatrix, voltages);

    /// <summary>
    /// Oblicza zespolone składowe pola elektrycznego, wartość skuteczną i
    /// półosie elipsy pola w jednym punkcie obserwacyjnym.
    ///
    /// Wzór (ujemny gradient potencjału skalarnego, z korekcją obrazu w ziemi):
    ///
    ///   Ex = (1/2πε₀) · Σ qk · [(x−xk)/Rk² − (x−xk)/R'k²]
    ///   Ey = (1/2πε₀) · Σ qk · [(y−yk)/Rk² − (y+yk)/R'k²]
    ///
    /// gdzie Rk  = odległość do rzeczywistego przewodu k
    ///       R'k = odległość do obrazu przewodu k (położonego przy y = −yk)
    ///
    /// Wartość skuteczna: E = √(|Ex|² + |Ey|²)
    /// Półosie elipsy Ea ≥ Eb obliczane przez FieldEllipseCalculator.
    /// </summary>
    public (Complex Ex, Complex Ey, double E, double Ea, double Eb) CalculateAtPoint(
        IReadOnlyList<Conductor> electricConductors,
        Complex[] lineCharges,
        ObservationPoint point)
    {
        Complex ex = Complex.Zero;
        Complex ey = Complex.Zero;
        double coeff = 1.0 / (2.0 * Math.PI * PhysicalConstants.Epsilon0);

        for (int k = 0; k < electricConductors.Count; k++)
        {
            var c = electricConductors[k];
            Complex q = lineCharges[k];

            double dx      = point.X - c.X;
            double dyReal  = point.Y - c.Y;   // Δy do rzeczywistego przewodu
            double dyImage = point.Y + c.Y;   // Δy do obrazu przewodu (przy −yk)

            double rReal2  = dx * dx + dyReal  * dyReal;   // Rk²
            double rImage2 = dx * dx + dyImage * dyImage;  // R'k²

            ex += q * (dx     / rReal2 - dx     / rImage2);
            ey += q * (dyReal / rReal2 - dyImage / rImage2);
        }

        ex *= coeff;
        ey *= coeff;

        // E = √(|Ex|² + |Ey|²)  – wartość skuteczna (niezmieniona)
        double e = Math.Sqrt(ex.Magnitude * ex.Magnitude + ey.Magnitude * ey.Magnitude);

        // Półosie elipsy opisywanej przez wektor pola elektrycznego w ciągu jednego okresu
        var (ea, eb) = FieldEllipseCalculator.CalculateAxes(ex, ey);

        return (ex, ey, e, ea, eb);
    }
}

