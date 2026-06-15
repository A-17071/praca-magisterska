namespace ModulKalkulacyjny.Domain;

/// <summary>
/// Punkt w dwuwymiarowym przekroju poprzecznym, w którym obliczane jest pole.
/// x – współrzędna pozioma [m], y – wysokość nad ziemią [m].
/// </summary>
public readonly record struct ObservationPoint(double X, double Y);

