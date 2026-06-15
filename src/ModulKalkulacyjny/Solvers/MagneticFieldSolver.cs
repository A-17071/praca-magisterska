using System.Numerics;
using ModulKalkulacyjny.Domain;
using ModulKalkulacyjny.Helpers;

namespace ModulKalkulacyjny.Solvers;

/// <summary>
/// Oblicza indukcję magnetyczną B i natężenie pola magnetycznego H
/// w punktach obserwacyjnych metodą Biota-Savarta dla nieskończonych przewodów liniowych.
///
///   Bx = −(μ₀/2π) · Σ Ik · (y−yk) / Rk²
///   By =  (μ₀/2π) · Σ Ik · (x−xk) / Rk²
///
/// gdzie Rk² = (x−xk)² + (y−yk)²
///
/// Natężenie pola magnetycznego (w powietrzu, μ ≈ μ₀):
///   Hx = Bx / μ₀,  Hy = By / μ₀,  H = B / μ₀
/// </summary>
public sealed class MagneticFieldSolver
{
    /// <summary>
    /// Oblicza B (indukcja magnetyczna), H (natężenie pola magnetycznego) oraz
    /// półosie obu elips pola w danym punkcie obserwacyjnym.
    /// </summary>
    public (Complex Bx, Complex By, double B, Complex Hx, Complex Hy, double H,
            double Ba, double Bb, double Ha, double Hb)
        CalculateAtPoint(IReadOnlyList<Conductor> conductors, ObservationPoint point)
    {
        Complex bx = Complex.Zero;
        Complex by = Complex.Zero;
        double coeff = PhysicalConstants.Mu0 / (2.0 * Math.PI);

        foreach (var c in conductors.Where(c => c.IncludeInMagneticField))
        {
            double dx = point.X - c.X;
            double dy = point.Y - c.Y;
            double r2 = dx * dx + dy * dy; // Rk²

            // Bx = −(μ₀/2π) · I · (y−yk) / Rk²
            bx += -coeff * c.Current * (dy / r2);
            // By =  (μ₀/2π) · I · (x−xk) / Rk²
            by +=  coeff * c.Current * (dx / r2);
        }

        // Wartość skuteczna: B = √(|Bx|² + |By|²)  (niezmieniona)
        double b = Math.Sqrt(bx.Magnitude * bx.Magnitude + by.Magnitude * by.Magnitude);

        // H = B / μ₀  (obowiązuje w powietrzu gdzie μ ≈ μ₀)
        Complex hx = bx / PhysicalConstants.Mu0;
        Complex hy = by / PhysicalConstants.Mu0;
        double  h  = b  / PhysicalConstants.Mu0;

        // Półosie elipsy opisywanej przez wektor pola magnetycznego w ciągu jednego okresu
        var (ba, bb) = FieldEllipseCalculator.CalculateAxes(bx, by);
        double ha = ba / PhysicalConstants.Mu0;
        double hb = bb / PhysicalConstants.Mu0;

        return (bx, by, b, hx, hy, h, ba, bb, ha, hb);
    }
}

