using System.Numerics;

namespace ModulKalkulacyjny.Domain;

/// <summary>
/// One time interval during which conductor voltage and current phasors are constant.
/// Used by the exposure dose calculator to accumulate EDEP = Σ K^p · Δt.
/// </summary>
public sealed class OperatingState
{
    /// <summary>Duration of this interval [s]. Must be positive.</summary>
    public double DurationSeconds { get; set; }

    /// <summary>
    /// RMS voltage phasors for each conductor, in the same order as the conductor list [V].
    /// </summary>
    public IReadOnlyList<Complex> Voltages { get; set; } = Array.Empty<Complex>();

    /// <summary>
    /// RMS current phasors for each conductor, in the same order as the conductor list [A].
    /// </summary>
    public IReadOnlyList<Complex> Currents { get; set; } = Array.Empty<Complex>();
}

