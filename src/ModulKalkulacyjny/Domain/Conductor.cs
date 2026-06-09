using System.Numerics;

namespace ModulKalkulacyjny.Domain;

/// <summary>
/// Represents a single conductor in the overhead power line cross-section model.
/// Stores both geometry and electrical state (RMS phasors).
/// </summary>
public sealed class Conductor
{
    /// <summary>User-defined label for the conductor (e.g. "A", "B", "C").</summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>Horizontal coordinate [m].</summary>
    public double X { get; set; }

    /// <summary>Height above ground [m]. Must be greater than the radius.</summary>
    public double Y { get; set; }

    /// <summary>Conductor radius or equivalent bundle radius [m].</summary>
    public double Radius { get; set; }

    /// <summary>Phase-to-ground RMS voltage phasor [V].</summary>
    public Complex Voltage { get; set; }

    /// <summary>RMS current phasor [A].</summary>
    public Complex Current { get; set; }

    /// <summary>Whether this conductor is included in the electric field calculation.</summary>
    public bool IncludeInElectricField { get; set; } = true;

    /// <summary>Whether this conductor is included in the magnetic field calculation.</summary>
    public bool IncludeInMagneticField { get; set; } = true;
}

