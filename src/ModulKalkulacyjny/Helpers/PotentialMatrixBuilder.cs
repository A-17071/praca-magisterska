using ModulKalkulacyjny.Domain;

namespace ModulKalkulacyjny.Helpers;

/// <summary>
/// Buduje macierz współczynników potencjału Maxwella P używaną w obliczeniach pola elektrycznego.
/// Macierz wiąże liniowe gęstości ładunku z napięciami przewodów: U = P · q.
/// </summary>
public sealed class PotentialMatrixBuilder
{
    /// <summary>
    /// Buduje rzeczywistą symetryczną macierz n×n dla przewodów z IncludeInElectricField = true.
    ///
    /// Diagonalna   (potencjał własny):    Pkk  = (1 / 2πε₀) · ln(2·yk / rk)
    /// Pozadiagonalna (potencjał wzajemny): Plk = (1 / 2πε₀) · ln(D'lk / Dlk)
    ///
    /// gdzie:
    ///   Dlk  = rzeczywista odległość między przewodem l a przewodem k
    ///   D'lk = odległość między przewodem l a obrazem przewodu k (przy y = −yk)
    /// </summary>
    public double[,] Build(IReadOnlyList<Conductor> conductors)
    {
        var ec = conductors.Where(c => c.IncludeInElectricField).ToList();
        int n = ec.Count;

        if (n == 0)
            throw new ArgumentException("Żaden przewód nie jest uwzględniony w obliczeniach pola elektrycznego.");

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
                    // Potencjał własny: obraz przewodu jest w (xk, −yk), odległość = 2·yk od rzeczywistego przewodu
                    p[l, k] = coeff * Math.Log(2.0 * ck.Y / ck.Radius);
                }
                else
                {
                    double dx    = cl.X - ck.X;
                    // Dlk  – odległość między dwoma rzeczywistymi przewodami
                    double dReal = Math.Sqrt(dx * dx + Math.Pow(cl.Y - ck.Y, 2));
                    // D'lk – odległość od przewodu l do obrazu przewodu k
                    double dImg  = Math.Sqrt(dx * dx + Math.Pow(cl.Y + ck.Y, 2));

                    p[l, k] = coeff * Math.Log(dImg / dReal);
                }
            }
        }

        return p;
    }
}

