namespace ModulKalkulacyjny;

/// <summary>
/// Builds the Maxwell potential coefficient matrix P used in the electric field calculation.
/// The matrix relates line charge densities to conductor voltages: U = P · q.
/// </summary>
public sealed class PotentialMatrixBuilder
{
    /// <summary>
    /// Builds an n×n real symmetric matrix for the conductors with IncludeInElectricField = true.
    ///
    /// Diagonal   (self-potential):   Pkk  = (1 / 2πε₀) · ln(2·yk / rk)
    /// Off-diagonal (mutual potential): Plk = (1 / 2πε₀) · ln(D'lk / Dlk)
    ///
    /// where:
    ///   Dlk  = real distance between conductor l and conductor k
    ///   D'lk = distance between conductor l and the image of conductor k (at y = −yk)
    /// </summary>
    public double[,] Build(IReadOnlyList<Conductor> conductors)
    {
        var ec = conductors.Where(c => c.IncludeInElectricField).ToList();
        int n = ec.Count;

        if (n == 0)
            throw new ArgumentException("No conductors are included in the electric field calculation.");

        var p = new double[n, n];
        double coeff = 1.0 / (2.0 * Math.PI * PhysicalConstants.Epsilon0);

        for (int l = 0; l < n; l++)
        {
            for (int k = 0; k < n; k++)
            {
                var cl = ec[l];
                var ck = ec[k];

                if (l == k)
                {
                    // Self-potential: image conductor is at (xk, −yk), distance = 2·yk from real conductor
                    p[l, k] = coeff * Math.Log(2.0 * ck.Y / ck.Radius);
                }
                else
                {
                    double dx    = cl.X - ck.X;
                    // Dlk  – distance between two real conductors
                    double dReal = Math.Sqrt(dx * dx + Math.Pow(cl.Y - ck.Y, 2));
                    // D'lk – distance from conductor l to the image of conductor k
                    double dImg  = Math.Sqrt(dx * dx + Math.Pow(cl.Y + ck.Y, 2));

                    p[l, k] = coeff * Math.Log(dImg / dReal);
                }
            }
        }

        return p;
    }
}

