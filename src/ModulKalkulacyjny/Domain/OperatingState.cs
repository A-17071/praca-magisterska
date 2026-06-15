using System.Numerics;

namespace ModulKalkulacyjny.Domain;

/// <summary>
/// Jeden przedział czasu, w którym fazory napięcia i prądu przewodów są stałe.
/// Używany przez kalkulator dawki ekspozycji do akumulowania EDEP = Σ K^p · Δt.
/// </summary>
public sealed class OperatingState
{
    /// <summary>Czas trwania przedziału [s]. Musi być dodatni.</summary>
    public double DurationSeconds { get; set; }

    /// <summary>
    /// Fazory napięcia RMS dla każdego przewodu, w tej samej kolejności co lista przewodów [V].
    /// </summary>
    public IReadOnlyList<Complex> Voltages { get; set; } = Array.Empty<Complex>();

    /// <summary>
    /// Fazory prądu RMS dla każdego przewodu, w tej samej kolejności co lista przewodów [A].
    /// </summary>
    public IReadOnlyList<Complex> Currents { get; set; } = Array.Empty<Complex>();
}

