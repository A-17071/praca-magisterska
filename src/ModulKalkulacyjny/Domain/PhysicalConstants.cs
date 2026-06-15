namespace ModulKalkulacyjny.Domain;

/// <summary>Stałe fizyczne używane w obliczeniach pola elektromagnetycznego.</summary>
public static class PhysicalConstants
{
    /// <summary>Przenikalność elektryczna próżni ε₀ [F/m]</summary>
    public const double Epsilon0 = 8.854187817e-12;

    /// <summary>Przenikalność magnetyczna próżni μ₀ [H/m]</summary>
    public const double Mu0 = 4.0 * Math.PI * 1e-7;
}

