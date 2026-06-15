namespace ModulKalkulacyjny.Domain;

/// <summary>
/// Wyniki dawki ekspozycji w jednym punkcie obserwacyjnym, skumulowane dla wszystkich stanów pracy.
/// EDEP = Σ K^p · Δt  (energetyczna dawka ekspozycji)
/// </summary>
public sealed class ExposureResult
{
    /// <summary>Punkt obserwacyjny, w którym obliczono ekspozycję.</summary>
    public ObservationPoint Point { get; set; }

    // ── Dawki ekspozycji ─────────────────────────────────────────────────────

    /// <summary>Dawka ekspozycji pola elektrycznego  EDEPE = Σ E^p · Δt [V^p·m^-p·s].</summary>
    public double Edepe { get; set; }

    /// <summary>Dawka ekspozycji indukcji magnetycznej  EDEPM_B = Σ B^p · Δt [T^p·s].</summary>
    public double EdepmB { get; set; }

    /// <summary>Dawka ekspozycji natężenia pola magnetycznego  EDEPM_H = Σ H^p · Δt [(A/m)^p·s].</summary>
    public double EdepmH { get; set; }

    // ── Minimum / Maksimum dla wszystkich stanów ─────────────────────────────

    public double EMax { get; set; }
    public double EMin { get; set; }
    public double BMax { get; set; }
    public double BMin { get; set; }
    public double HMax { get; set; }
    public double HMin { get; set; }

    // ── Średnie ważone czasowo ───────────────────────────────────────────────

    /// <summary>Średnia ważona czasowo pola elektrycznego [V/m].</summary>
    public double EAverage { get; set; }

    /// <summary>Średnia ważona czasowo indukcji magnetycznej [T].</summary>
    public double BAverage { get; set; }

    /// <summary>Średnia ważona czasowo natężenia pola magnetycznego [A/m].</summary>
    public double HAverage { get; set; }

    // ── Równoważne wartości stałego pola ─────────────────────────────────────

    /// <summary>Równoważne stałe pole elektryczne Eeq = (EDEPE / T)^(1/p) [V/m].</summary>
    public double EEquivalent { get; set; }

    /// <summary>Równoważna stała indukcja magnetyczna Beq = (EDEPM_B / T)^(1/p) [T].</summary>
    public double BEquivalent { get; set; }

    /// <summary>Równoważne stałe natężenie pola magnetycznego Heq = (EDEPM_H / T)^(1/p) [A/m].</summary>
    public double HEquivalent { get; set; }
}

