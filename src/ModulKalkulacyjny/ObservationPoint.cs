namespace ModulKalkulacyjny;

/// <summary>
/// A point in the 2-D cross-section where the field is to be evaluated.
/// x is horizontal [m], y is height above ground [m].
/// </summary>
public readonly record struct ObservationPoint(double X, double Y);

