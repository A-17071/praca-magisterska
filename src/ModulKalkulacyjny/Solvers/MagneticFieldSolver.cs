using System.Numerics;
using ModulKalkulacyjny.Domain;

namespace ModulKalkulacyjny.Solvers;

/// <summary>
/// Calculates the magnetic flux density B and magnetic field strength H
/// at observation points using the Biot-Savart law for infinite line currents.
///
///   Bx = −(μ₀/2π) · Σ Ik · (y−yk) / Rk²
///   By =  (μ₀/2π) · Σ Ik · (x−xk) / Rk²
///
/// where Rk² = (x−xk)² + (y−yk)²
///
/// Magnetic field strength (in air, μ ≈ μ₀):
///   Hx = Bx / μ₀,  Hy = By / μ₀,  H = B / μ₀
/// </summary>
public sealed class MagneticFieldSolver
{
    /// <summary>
    /// Calculates B (magnetic flux density) and H (magnetic field strength)
    /// at the given observation point from all conductors with IncludeInMagneticField = true.
    /// </summary>
    public (Complex Bx, Complex By, double B, Complex Hx, Complex Hy, double H)
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

        // RMS magnitude: B = √(|Bx|² + |By|²)
        double b = Math.Sqrt(bx.Magnitude * bx.Magnitude + by.Magnitude * by.Magnitude);

        // H = B / μ₀  (valid in air where μ ≈ μ₀)
        Complex hx = bx / PhysicalConstants.Mu0;
        Complex hy = by / PhysicalConstants.Mu0;
        double  h  = b  / PhysicalConstants.Mu0;

        return (bx, by, b, hx, hy, h);
    }
}

