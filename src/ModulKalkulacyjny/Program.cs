using ModulKalkulacyjny.Domain;
using ModulKalkulacyjny.Solvers;

namespace ModulKalkulacyjny;

/// <summary>
/// Ready-made test conductors matching the balanced three-phase scenario
/// from the specification (section 20).
/// Useful for quick verification and UI pre-population.
/// </summary>
public static class TestCases
{
    /// <summary>
    /// Returns three balanced phase conductors at y = 12 m with the given RMS voltage and current.
    /// Default voltage is 110 kV line-to-line converted to phase-to-ground: 110 000 / sqrt(3) ≈ 63 509 V.
    /// </summary>
    public static IReadOnlyList<Conductor> BalancedThreePhase(
        double voltageRms = 63_509.0,
        double currentRms = 400.0)
    {
        return new[]
        {
            new Conductor
            {
                Id = "A", X = -4, Y = 12, Radius = 0.015,
                Voltage = ElectromagneticFieldCalculator.Phasor(voltageRms,    0),
                Current = ElectromagneticFieldCalculator.Phasor(currentRms,    0)
            },
            new Conductor
            {
                Id = "B", X =  0, Y = 12, Radius = 0.015,
                Voltage = ElectromagneticFieldCalculator.Phasor(voltageRms, -120),
                Current = ElectromagneticFieldCalculator.Phasor(currentRms, -120)
            },
            new Conductor
            {
                Id = "C", X =  4, Y = 12, Radius = 0.015,
                Voltage = ElectromagneticFieldCalculator.Phasor(voltageRms,  120),
                Current = ElectromagneticFieldCalculator.Phasor(currentRms,  120)
            },
        };
    }
}
