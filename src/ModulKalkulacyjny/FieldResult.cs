using System.Numerics;

namespace ModulKalkulacyjny;

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
}

