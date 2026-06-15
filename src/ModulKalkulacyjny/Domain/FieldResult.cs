using System.Numerics;

namespace ModulKalkulacyjny.Domain;

/// <summary>
/// All field components and magnitudes calculated at a single observation point
/// for one set of conductor voltages and currents.
/// </summary>
public sealed class FieldResult
{
    /// <summary>The observation point where the field was calculated.</summary>
    public ObservationPoint Point { get; set; }

    // ── Electric field ──────────────────────────────────────────────────────

    /// <summary>Complex horizontal electric field component Ex [V/m].</summary>
    public Complex Ex { get; set; }

    /// <summary>Complex vertical electric field component Ey [V/m].</summary>
    public Complex Ey { get; set; }

    /// <summary>RMS electric field magnitude E = √(|Ex|² + |Ey|²) [V/m].</summary>
    public double E { get; set; }

    // ── Magnetic flux density ────────────────────────────────────────────────

    /// <summary>Complex horizontal magnetic flux density component Bx [T].</summary>
    public Complex Bx { get; set; }

    /// <summary>Complex vertical magnetic flux density component By [T].</summary>
    public Complex By { get; set; }

    /// <summary>RMS magnetic flux density magnitude B = √(|Bx|² + |By|²) [T].</summary>
    public double B { get; set; }

    // ── Magnetic field strength ──────────────────────────────────────────────

    /// <summary>Complex horizontal magnetic field strength component Hx [A/m].</summary>
    public Complex Hx { get; set; }

    /// <summary>Complex vertical magnetic field strength component Hy [A/m].</summary>
    public Complex Hy { get; set; }

    /// <summary>RMS magnetic field strength magnitude H = B / μ₀ [A/m].</summary>
    public double H { get; set; }

    // ── Electric-field ellipse semi-axes ─────────────────────────────────────

    /// <summary>
    /// RMS longer semi-axis of the electric-field ellipse [V/m].
    /// Calculated from the complex phasor components Ex and Ey.
    /// Describes the maximum effective magnitude of the rotating electric-field vector.
    /// </summary>
    public double Ea { get; set; }

    /// <summary>
    /// RMS shorter semi-axis of the electric-field ellipse [V/m].
    /// Calculated from the complex phasor components Ex and Ey.
    /// Describes the minimum effective magnitude of the rotating electric-field vector.
    /// </summary>
    public double Eb { get; set; }

    /// <summary>
    /// Diagnostic verification value reconstructed from the electric-field ellipse semi-axes:
    /// EFromAxes = √(Ea² + Eb²) [V/m].
    /// Used only to check consistency between the total RMS electric field and its ellipse representation.
    /// </summary>
    public double EFromAxes { get; set; }

    /// <summary>
    /// Diagnostic verification difference for the electric field:
    /// EDifference = |E − EFromAxes| [V/m].
    /// Used only to check consistency between the total RMS electric field and its ellipse representation.
    /// </summary>
    public double EDifference { get; set; }

    // ── Magnetic-flux-density ellipse semi-axes ──────────────────────────────

    /// <summary>
    /// RMS longer semi-axis of the magnetic-flux-density ellipse [T].
    /// Calculated from the complex phasor components Bx and By.
    /// </summary>
    public double Ba { get; set; }

    /// <summary>
    /// RMS shorter semi-axis of the magnetic-flux-density ellipse [T].
    /// Calculated from the complex phasor components Bx and By.
    /// </summary>
    public double Bb { get; set; }

    /// <summary>
    /// Diagnostic verification value reconstructed from the magnetic-flux-density ellipse semi-axes:
    /// BFromAxes = √(Ba² + Bb²) [T].
    /// Used only to check consistency between the total RMS magnetic flux density and its ellipse representation.
    /// </summary>
    public double BFromAxes { get; set; }

    /// <summary>
    /// Diagnostic verification difference for the magnetic flux density:
    /// BDifference = |B − BFromAxes| [T].
    /// Used only to check consistency between the total RMS magnetic flux density and its ellipse representation.
    /// </summary>
    public double BDifference { get; set; }

    // ── Magnetic-field-strength ellipse semi-axes ────────────────────────────

    /// <summary>
    /// RMS longer semi-axis of the magnetic-field-strength ellipse [A/m].
    /// Ha = Ba / μ₀.
    /// </summary>
    public double Ha { get; set; }

    /// <summary>
    /// RMS shorter semi-axis of the magnetic-field-strength ellipse [A/m].
    /// Hb = Bb / μ₀.
    /// </summary>
    public double Hb { get; set; }

    /// <summary>
    /// Diagnostic verification value reconstructed from the magnetic-field-strength ellipse semi-axes:
    /// HFromAxes = √(Ha² + Hb²) [A/m].
    /// Used only to check consistency between the total RMS magnetic field strength and its ellipse representation.
    /// </summary>
    public double HFromAxes { get; set; }

    /// <summary>
    /// Diagnostic verification difference for the magnetic field strength:
    /// HDifference = |H − HFromAxes| [A/m].
    /// Used only to check consistency between the total RMS magnetic field strength and its ellipse representation.
    /// </summary>
    public double HDifference { get; set; }
}

