namespace ModulKalkulacyjny.Domain;

/// <summary>
/// Exposure dose results at a single observation point, accumulated over all operating states.
/// EDEP = Σ K^p · Δt  (energetic exposure dose)
/// </summary>
public sealed class ExposureResult
{
    /// <summary>The observation point where the exposure was calculated.</summary>
    public ObservationPoint Point { get; set; }

    // ── Exposure doses ───────────────────────────────────────────────────────

    /// <summary>Electric field exposure dose  EDEPE = Σ E^p · Δt [V^p·m^-p·s].</summary>
    public double Edepe { get; set; }

    /// <summary>Magnetic flux density exposure dose  EDEPM_B = Σ B^p · Δt [T^p·s].</summary>
    public double EdepmB { get; set; }

    /// <summary>Magnetic field strength exposure dose  EDEPM_H = Σ H^p · Δt [(A/m)^p·s].</summary>
    public double EdepmH { get; set; }

    // ── Min / Max over all states ────────────────────────────────────────────

    public double EMax { get; set; }
    public double EMin { get; set; }
    public double BMax { get; set; }
    public double BMin { get; set; }
    public double HMax { get; set; }
    public double HMin { get; set; }

    // ── Time-weighted averages ───────────────────────────────────────────────

    /// <summary>Time-weighted average electric field [V/m].</summary>
    public double EAverage { get; set; }

    /// <summary>Time-weighted average magnetic flux density [T].</summary>
    public double BAverage { get; set; }

    /// <summary>Time-weighted average magnetic field strength [A/m].</summary>
    public double HAverage { get; set; }

    // ── Equivalent constant field values ────────────────────────────────────

    /// <summary>Equivalent constant electric field Eeq = (EDEPE / T)^(1/p) [V/m].</summary>
    public double EEquivalent { get; set; }

    /// <summary>Equivalent constant magnetic flux density Beq = (EDEPM_B / T)^(1/p) [T].</summary>
    public double BEquivalent { get; set; }

    /// <summary>Equivalent constant magnetic field strength Heq = (EDEPM_H / T)^(1/p) [A/m].</summary>
    public double HEquivalent { get; set; }
}

