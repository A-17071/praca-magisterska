using System.Numerics;

namespace ModulKalkulacyjny.Solvers;

/// <summary>
/// Rozwiązuje rzeczywisty kwadratowy układ liniowy A · x = b, gdzie b (i zatem x) jest wektorem zespolonym.
///
/// Strategia: ponieważ A jest rzeczywista, układ rozdziela się na dwa rzeczywiste układy:
///   A · Re(x) = Re(b)
///   A · Im(x) = Im(b)
/// Oba są rozwiązywane przy użyciu tej samej faktoryzacji LU (eliminacja Gaussa z częściowym wyborem pivota).
/// </summary>
public sealed class LinearSystemSolver
{
    /// <summary>
    /// Rozwiązuje A · x = b i zwraca x.
    /// Zgłasza <see cref="InvalidOperationException"/> gdy macierz jest osobliwa.
    /// </summary>
    public Complex[] Solve(double[,] matrix, Complex[] rhs)
    {
        int n = rhs.Length;

        if (matrix.GetLength(0) != n || matrix.GetLength(1) != n)
            throw new ArgumentException("Wymiary macierzy muszą odpowiadać długości wektora prawej strony.");

        // Rozdziel zespoloną prawą stronę na części rzeczywistą i urojoną
        double[] rhsReal = new double[n];
        double[] rhsImag = new double[n];
        for (int i = 0; i < n; i++)
        {
            rhsReal[i] = rhs[i].Real;
            rhsImag[i] = rhs[i].Imaginary;
        }

        // Rozwiąż obie połowy przy użyciu tej samej macierzy rzeczywistej
        double[] xReal = GaussElim((double[,])matrix.Clone(), rhsReal);
        double[] xImag = GaussElim((double[,])matrix.Clone(), rhsImag);

        var result = new Complex[n];
        for (int i = 0; i < n; i++)
            result[i] = new Complex(xReal[i], xImag[i]);

        return result;
    }

    // ── Eliminacja Gaussa z częściowym wyborem pivota (wierszowym) ───────────

    private static double[] GaussElim(double[,] a, double[] b)
    {
        int n = b.Length;
        b = (double[])b.Clone(); // praca na kopii

        // Eliminacja w przód
        for (int col = 0; col < n; col++)
        {
            // Znajdź wiersz z największą wartością bezwzględną w tej kolumnie (pivot)
            int pivotRow = col;
            double pivotVal = Math.Abs(a[col, col]);
            for (int row = col + 1; row < n; row++)
            {
                double val = Math.Abs(a[row, col]);
                if (val > pivotVal) { pivotVal = val; pivotRow = row; }
            }

            if (pivotVal < 1e-14)
                throw new InvalidOperationException(
                    "Macierz współczynników potencjału jest osobliwa lub prawie osobliwa. " +
                    "Sprawdź, czy promienie i wysokości przewodów są poprawne.");

            // Zamień wiersze jeśli potrzeba
            if (pivotRow != col)
            {
                for (int k = 0; k < n; k++)
                    (a[col, k], a[pivotRow, k]) = (a[pivotRow, k], a[col, k]);
                (b[col], b[pivotRow]) = (b[pivotRow], b[col]);
            }

            // Wyzeruj elementy poniżej pivota
            for (int row = col + 1; row < n; row++)
            {
                double factor = a[row, col] / a[col, col];
                for (int k = col; k < n; k++)
                    a[row, k] -= factor * a[col, k];
                b[row] -= factor * b[col];
            }
        }

        // Podstawianie wsteczne
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

