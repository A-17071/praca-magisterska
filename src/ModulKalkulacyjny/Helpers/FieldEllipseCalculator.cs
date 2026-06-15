using System.Numerics;

namespace ModulKalkulacyjny.Helpers;

/// <summary>
/// Calculates the RMS semi-axes of the ellipse traced by a 2-D sinusoidal field vector
/// during one period.
///
/// When the horizontal and vertical components of a field have different phase angles
/// the tip of the field vector traces an ellipse. This class implements the semi-axis
/// calculation using the decomposition into two oppositely rotating circular components
/// F1 and F2, as given in the reference literature:
///
///   F1 = 0.5 · (Fy + j·Fx)          — positive-rotation component
///   F2 = 0.5 · (conj(Fy) + j·conj(Fx)) — negative-rotation component
///
///   MajorAxis = |F1| + |F2|
///   MinorAxis = ||F1| − |F2||
///
/// The two circular components have the same magnitude only when the field vector
/// traces a circle (MinorAxis = 0). They are equal in magnitude and anti-phase only
/// when the field is linearly polarised (MinorAxis = MajorAxis / √2 in that limit).
/// </summary>
public static class FieldEllipseCalculator
{
    /// <summary>
    /// Returns the RMS longer (major) and shorter (minor) semi-axes of the field ellipse.
    ///
    /// The method follows the book notation:
    ///   F1 = 0.5 * (Fy  +  j * Fx)
    ///   F2 = 0.5 * (Fy* +  j * Fx*)
    ///   MajorAxis = |F1| + |F2|
    ///   MinorAxis = ||F1| - |F2||
    ///
    /// Usage:
    ///   Electric field  — call with (Ex, Ey), result = (Ea, Eb) [V/m]
    ///   Magnetic field  — call with (Bx, By), result = (Ba, Bb) [T]
    /// </summary>
    /// <param name="fx">Complex RMS phasor of the horizontal (x) component.</param>
    /// <param name="fy">Complex RMS phasor of the vertical   (y) component.</param>
    /// <returns>(MajorAxis, MinorAxis) in the same units as the input phasors.</returns>
    public static (double MajorAxis, double MinorAxis) CalculateAxes(Complex fx, Complex fy)
    {
        Complex j = Complex.ImaginaryOne;

        // Positive-rotation circular component
        Complex f1 = 0.5 * (fy + j * fx);

        // Negative-rotation circular component (uses conjugates)
        Complex f2 = 0.5 * (Complex.Conjugate(fy) + j * Complex.Conjugate(fx));

        double f1Mag = f1.Magnitude;
        double f2Mag = f2.Magnitude;

        // The longer semi-axis is the sum of the two circular amplitudes;
        // the shorter semi-axis is their absolute difference.
        double majorAxis = f1Mag + f2Mag;
        double minorAxis = Math.Abs(f1Mag - f2Mag);

        return (majorAxis, minorAxis);
    }
}
