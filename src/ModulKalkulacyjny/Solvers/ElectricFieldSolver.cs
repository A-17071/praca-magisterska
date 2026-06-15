using System.Numerics;
using ModulKalkulacyjny.Domain;
using ModulKalkulacyjny.Helpers;

namespace ModulKalkulacyjny.Solvers;

/// <summary>
/// Solves for line charge density phasors and calculates the electric field.
///
/// Two-step process:
///   1. Solve  P · q = U  →  line charge densities q
///   2. For each observation point: E = −∇V  (negative gradient of potential)
/// </summary>
public sealed class ElectricFieldSolver
{
    private readonly LinearSystemSolver _linSolver = new();

    /// <summary>
    /// Solves the linear system P · q = U to obtain line charge density phasors [C/m].
    /// </summary>
    public Complex[] SolveLineCharges(double[,] potentialMatrix, Complex[] voltages)
        => _linSolver.Solve(potentialMatrix, voltages);

    /// <summary>
    /// Calculates the complex electric field components, RMS magnitude, and
    /// the semi-axes of the field ellipse at one observation point.
    ///
    /// Formula (negative gradient of the scalar potential, including ground-image correction):
    ///
    ///   Ex = (1/2πε₀) · Σ qk · [(x−xk)/Rk² − (x−xk)/R'k²]
    ///   Ey = (1/2πε₀) · Σ qk · [(y−yk)/Rk² − (y+yk)/R'k²]
    ///
    /// where Rk  = distance to the real conductor k
    ///       R'k = distance to the image conductor k (located at y = −yk)
    ///
    /// RMS magnitude: E = √(|Ex|² + |Ey|²)
    /// Ellipse semi-axes Ea ≥ Eb are calculated by FieldEllipseCalculator.
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
            double dyReal  = point.Y - c.Y;   // Δy to real conductor
            double dyImage = point.Y + c.Y;   // Δy to image conductor (at −yk)

            double rReal2  = dx * dx + dyReal  * dyReal;   // Rk²
            double rImage2 = dx * dx + dyImage * dyImage;  // R'k²

            ex += q * (dx     / rReal2 - dx     / rImage2);
            ey += q * (dyReal / rReal2 - dyImage / rImage2);
        }

        ex *= coeff;
        ey *= coeff;

        // E = √(|Ex|² + |Ey|²)  – RMS magnitude (unchanged)
        double e = Math.Sqrt(ex.Magnitude * ex.Magnitude + ey.Magnitude * ey.Magnitude);

        // Semi-axes of the ellipse traced by the electric-field vector during one period
        var (ea, eb) = FieldEllipseCalculator.CalculateAxes(ex, ey);

        return (ex, ey, e, ea, eb);
    }
}

