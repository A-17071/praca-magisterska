using System.Numerics;

namespace ModulKalkulacyjny.Domain;

/// <summary>
/// Reprezentuje pojedynczy przewód w modelu przekroju poprzecznego napowietrznej linii energetycznej.
/// Przechowuje zarówno geometrię, jak i stan elektryczny (fazory RMS).
/// </summary>
public sealed class Conductor
{
    /// <summary>Etykieta przewodu zdefiniowana przez użytkownika (np. „A", „B", „C").</summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>Współrzędna pozioma [m].</summary>
    public double X { get; set; }

    /// <summary>Wysokość nad ziemią [m]. Musi być większa niż promień.</summary>
    public double Y { get; set; }

    /// <summary>Promień przewodu lub zastępczy promień wiązki [m].</summary>
    public double Radius { get; set; }

    /// <summary>Fasor napięcia fazowo-ziemnego RMS [V].</summary>
    public Complex Voltage { get; set; }

    /// <summary>Fasor prądu RMS [A].</summary>
    public Complex Current { get; set; }

    /// <summary>Czy przewód jest uwzględniany w obliczeniach pola elektrycznego.</summary>
    public bool IncludeInElectricField { get; set; } = true;

    /// <summary>Czy przewód jest uwzględniany w obliczeniach pola magnetycznego.</summary>
    public bool IncludeInMagneticField { get; set; } = true;
}

