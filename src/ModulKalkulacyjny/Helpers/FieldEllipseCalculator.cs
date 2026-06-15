using System.Numerics;

namespace ModulKalkulacyjny.Helpers;

/// <summary>
/// Oblicza półosie RMS elipsy opisywanej przez dwuwymiarowy sinusoidalny wektor pola
/// w ciągu jednego okresu.
///
/// Gdy pozioma i pionowa składowa pola mają różne kąty fazowe,
/// koniec wektora pola opisuje elipsę. Klasa implementuje obliczanie półosi
/// metodą rozkładu na dwa przeciwbieżnie obracające się składniki kołowe
/// F1 i F2, zgodnie z literaturą:
///
///   F1 = 0.5 · (Fy + j·Fx)                       — składnik o dodatnim kierunku obrotu
///   F2 = 0.5 · (conj(Fy) + j·conj(Fx))           — składnik o ujemnym kierunku obrotu
///
///   MajorAxis = |F1| + |F2|
///   MinorAxis = ||F1| − |F2||
///
/// Oba składniki mają jednakową amplitudę tylko wtedy, gdy wektor pola opisuje okrąg
/// (MinorAxis = 0). Są równe i przeciwfazowe tylko dla pola liniowo spolaryzowanego.
/// </summary>
public static class FieldEllipseCalculator
{
    /// <summary>
    /// Zwraca dłuższą (major) i krótszą (minor) półoś RMS elipsy pola.
    ///
    /// Metoda stosuje notację z literatury:
    ///   F1 = 0.5 * (Fy  +  j * Fx)
    ///   F2 = 0.5 * (Fy* +  j * Fx*)
    ///   MajorAxis = |F1| + |F2|
    ///   MinorAxis = ||F1| - |F2||
    ///
    /// Użycie:
    ///   Pole elektryczne  — wywołaj z (Ex, Ey), wynik = (Ea, Eb) [V/m]
    ///   Pole magnetyczne  — wywołaj z (Bx, By), wynik = (Ba, Bb) [T]
    /// </summary>
    /// <param name="fx">Zespolony fasor RMS poziomej składowej (x).</param>
    /// <param name="fy">Zespolony fasor RMS pionowej składowej (y).</param>
    /// <returns>(MajorAxis, MinorAxis) w tych samych jednostkach co dane wejściowe.</returns>
    public static (double MajorAxis, double MinorAxis) CalculateAxes(Complex fx, Complex fy)
    {
        Complex j = Complex.ImaginaryOne;

        // Składnik kołowy o dodatnim kierunku obrotu
        Complex f1 = 0.5 * (fy + j * fx);

        // Składnik kołowy o ujemnym kierunku obrotu (używa sprzężeń)
        Complex f2 = 0.5 * (Complex.Conjugate(fy) + j * Complex.Conjugate(fx));

        double f1Mag = f1.Magnitude;
        double f2Mag = f2.Magnitude;

        // Dłuższa półoś to suma amplitud dwóch składników kołowych;
        // krótsza półoś to ich wartość bezwzględna różnicy.
        double majorAxis = f1Mag + f2Mag;
        double minorAxis = Math.Abs(f1Mag - f2Mag);

        return (majorAxis, minorAxis);
    }
}
