using System.Numerics;

namespace ModulKalkulacyjny;

/// <summary>
/// Solves a real square linear system A · x = b where b (and therefore x) is a complex vector.
///
/// Strategy: since A is real, the system decouples into two real systems:
///   A · Re(x) = Re(b)
///   A · Im(x) = Im(b)
/// Both are solved with the same LU factorisation (Gaussian elimination + partial pivoting).
/// </summary>
public sealed class LinearSystemSolver
{
    /// <summary>
    /// Solves A · x = b and returns x.
    /// Throws <see cref="InvalidOperationException"/> if the matrix is singular.
    /// </summary>
    public Complex[] Solve(double[,] matrix, Complex[] rhs)
    {
        int n = rhs.Length;

        if (matrix.GetLength(0) != n || matrix.GetLength(1) != n)
            throw new ArgumentException("Matrix dimensions must match the RHS vector length.");

        // Split complex RHS into real and imaginary parts
        double[] rhsReal = new double[n];
        double[] rhsImag = new double[n];
        for (int i = 0; i < n; i++)
        {
            rhsReal[i] = rhs[i].Real;
            rhsImag[i] = rhs[i].Imaginary;
        }

        // Solve both halves with the same real matrix
        double[] xReal = GaussElim((double[,])matrix.Clone(), rhsReal);
        double[] xImag = GaussElim((double[,])matrix.Clone(), rhsImag);

        var result = new Complex[n];
        for (int i = 0; i < n; i++)
            result[i] = new Complex(xReal[i], xImag[i]);

        return result;
    }

    // ── Gaussian elimination with partial (row) pivoting ────────────────────

    private static double[] GaussElim(double[,] a, double[] b)
    {
        int n = b.Length;
        b = (double[])b.Clone(); // work on a copy

        // Forward elimination
        for (int col = 0; col < n; col++)
        {
            // Find the row with the largest absolute value in this column (pivot)
            int pivotRow = col;
            double pivotVal = Math.Abs(a[col, col]);
            for (int row = col + 1; row < n; row++)
            {
                double val = Math.Abs(a[row, col]);
                if (val > pivotVal) { pivotVal = val; pivotRow = row; }
            }

            if (pivotVal < 1e-14)
                throw new InvalidOperationException(
                    "Potential coefficient matrix is singular or nearly singular. " +
                    "Check that conductor radii and heights are valid.");

            // Swap rows if needed
            if (pivotRow != col)
            {
                for (int k = 0; k < n; k++)
                    (a[col, k], a[pivotRow, k]) = (a[pivotRow, k], a[col, k]);
                (b[col], b[pivotRow]) = (b[pivotRow], b[col]);
            }

            // Eliminate entries below the pivot
            for (int row = col + 1; row < n; row++)
            {
                double factor = a[row, col] / a[col, col];
                for (int k = col; k < n; k++)
                    a[row, k] -= factor * a[col, k];
                b[row] -= factor * b[col];
            }
        }

        // Back-substitution
        double[] x = new double[n];
        for (int i = n - 1; i >= 0; i--)
        {
            x[i] = b[i];
            for (int j = i + 1; j < n; j++)
                x[i] -= a[i, j] * x[j];
            x[i] /= a[i, i];
        }

        return x;
    }
}

