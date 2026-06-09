using ModulKalkulacyjny.Domain;

namespace ModulKalkulacyjny.Helpers;

/// <summary>
/// Calculates the equivalent bundle radius r_eq,k used in the diagonal term of the
/// electric-potential coefficient matrix:
///
///   P_kk = (1 / 2πε₀) · ln( 2·y_k / r_eq,k )
///
/// Each bundle is replaced by one equivalent conductor at the bundle centre.
/// The spacing parameter s_k is the distance between adjacent sub-conductors.
/// </summary>
public static class BundleCalculator
{
    /// <summary>Returns the number of sub-conductors b_k for a given bundle type.</summary>
    public static int SubconductorCount(BundleType bundle) => bundle switch
    {
        BundleType.Single         => 1,
        BundleType.Twin           => 2,
        BundleType.TripleTriangle => 3,
        BundleType.QuadSquare     => 4,
        _ => throw new ArgumentOutOfRangeException(nameof(bundle), "Unknown bundle type.")
    };

    /// <summary>
    /// Calculates the equivalent bundle radius r_eq,k [m].
    ///
    /// Formulas (r_s = sub-conductor radius, s = adjacent-conductor spacing):
    ///
    ///   Single:           r_eq = r_s
    ///   Twin:             r_eq = √(r_s · s)
    ///   Triple triangle:  r_eq = (r_s · s²)^(1/3)
    ///   Quad square:      r_eq = (√2 · r_s · s³)^(1/4)
    ///
    /// The result is passed as the Conductor.Radius to the PotentialMatrixBuilder.
    /// </summary>
    /// <param name="bundle">Bundle geometry type.</param>
    /// <param name="subRadius">Radius of one sub-conductor r_s [m]. Must be positive.</param>
    /// <param name="spacing">
    ///   Distance between adjacent sub-conductors s_k [m].
    ///   Ignored for Single; must be positive for all other types.
    /// </param>
    public static double CalculateEquivalentRadius(
        BundleType bundle,
        double subRadius,
        double spacing = 0)
    {
        if (subRadius <= 0)
            throw new ArgumentException(
                $"Sub-conductor radius must be positive (got {subRadius} m).", nameof(subRadius));

        if (bundle != BundleType.Single && spacing <= 0)
            throw new ArgumentException(
                $"Bundle spacing must be positive for {bundle} bundle (got {spacing} m).", nameof(spacing));

        return bundle switch
        {
            // r_eq = r_s  (trivial – single wire)
            BundleType.Single =>
                subRadius,

            // r_eq = √(r_s · s)
            BundleType.Twin =>
                Math.Sqrt(subRadius * spacing),

            // r_eq = (r_s · s²)^(1/3)
            BundleType.TripleTriangle =>
                Math.Pow(subRadius * spacing * spacing, 1.0 / 3.0),

            // r_eq = (√2 · r_s · s³)^(1/4)
            BundleType.QuadSquare =>
                Math.Pow(Math.Sqrt(2.0) * subRadius * Math.Pow(spacing, 3), 0.25),

            _ => throw new ArgumentOutOfRangeException(nameof(bundle), "Unknown bundle type.")
        };
    }
}

