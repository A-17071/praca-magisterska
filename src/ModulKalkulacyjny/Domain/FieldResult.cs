using System.Numerics;

namespace ModulKalkulacyjny.Domain;

/// <summary>
/// Wszystkie składowe pola i ich wartości skuteczne obliczone w jednym punkcie obserwacyjnym
/// dla jednego zestawu napięć i prądów przewodów.
/// </summary>
public sealed class FieldResult
{
    /// <summary>Punkt obserwacyjny, w którym obliczono pole.</summary>
    public ObservationPoint Point { get; set; }

    // ── Pole elektryczne ────────────────────────────────────────────────────

    /// <summary>Zespolona pozioma składowa pola elektrycznego Ex [V/m].</summary>
    public Complex Ex { get; set; }

    /// <summary>Zespolona pionowa składowa pola elektrycznego Ey [V/m].</summary>
    public Complex Ey { get; set; }

    /// <summary>Wartość skuteczna natężenia pola elektrycznego E = √(|Ex|² + |Ey|²) [V/m].</summary>
    public double E { get; set; }

    // ── Indukcja magnetyczna ─────────────────────────────────────────────────

    /// <summary>Zespolona pozioma składowa indukcji magnetycznej Bx [T].</summary>
    public Complex Bx { get; set; }

    /// <summary>Zespolona pionowa składowa indukcji magnetycznej By [T].</summary>
    public Complex By { get; set; }

    /// <summary>Wartość skuteczna indukcji magnetycznej B = √(|Bx|² + |By|²) [T].</summary>
    public double B { get; set; }

    // ── Natężenie pola magnetycznego ─────────────────────────────────────────

    /// <summary>Zespolona pozioma składowa natężenia pola magnetycznego Hx [A/m].</summary>
    public Complex Hx { get; set; }

    /// <summary>Zespolona pionowa składowa natężenia pola magnetycznego Hy [A/m].</summary>
    public Complex Hy { get; set; }

    /// <summary>Wartość skuteczna natężenia pola magnetycznego H = B / μ₀ [A/m].</summary>
    public double H { get; set; }

    // ── Półosie elipsy pola elektrycznego ────────────────────────────────────

    /// <summary>
    /// Dłuższa półoś RMS elipsy pola elektrycznego [V/m].
    /// Obliczana z zespolonych składowych fazorowych Ex i Ey.
    /// Opisuje maksymalną skuteczną wartość obracającego się wektora pola elektrycznego.
    /// </summary>
    public double Ea { get; set; }

    /// <summary>
    /// Krótsza półoś RMS elipsy pola elektrycznego [V/m].
    /// Obliczana z zespolonych składowych fazorowych Ex i Ey.
    /// Opisuje minimalną skuteczną wartość obracającego się wektora pola elektrycznego.
    /// </summary>
    public double Eb { get; set; }

    /// <summary>
    /// Diagnostyczna wartość weryfikacyjna odtworzona z półosi elipsy pola elektrycznego:
    /// EFromAxes = √(Ea² + Eb²) [V/m].
    /// Używana wyłącznie do sprawdzenia spójności między całkowitym polem elektrycznym RMS
    /// a jego reprezentacją eliptyczną.
    /// </summary>
    public double EFromAxes { get; set; }

    /// <summary>
    /// Diagnostyczna różnica weryfikacyjna dla pola elektrycznego:
    /// EDifference = |E − EFromAxes| [V/m].
    /// Używana wyłącznie do sprawdzenia spójności między całkowitym polem elektrycznym RMS
    /// a jego reprezentacją eliptyczną.
    /// </summary>
    public double EDifference { get; set; }

    // ── Półosie elipsy indukcji magnetycznej ─────────────────────────────────

    /// <summary>
    /// Dłuższa półoś RMS elipsy indukcji magnetycznej [T].
    /// Obliczana z zespolonych składowych fazorowych Bx i By.
    /// </summary>
    public double Ba { get; set; }

    /// <summary>
    /// Krótsza półoś RMS elipsy indukcji magnetycznej [T].
    /// Obliczana z zespolonych składowych fazorowych Bx i By.
    /// </summary>
    public double Bb { get; set; }

    /// <summary>
    /// Diagnostyczna wartość weryfikacyjna odtworzona z półosi elipsy indukcji magnetycznej:
    /// BFromAxes = √(Ba² + Bb²) [T].
    /// Używana wyłącznie do sprawdzenia spójności między całkowitą indukcją magnetyczną RMS
    /// a jej reprezentacją eliptyczną.
    /// </summary>
    public double BFromAxes { get; set; }

    /// <summary>
    /// Diagnostyczna różnica weryfikacyjna dla indukcji magnetycznej:
    /// BDifference = |B − BFromAxes| [T].
    /// Używana wyłącznie do sprawdzenia spójności między całkowitą indukcją magnetyczną RMS
    /// a jej reprezentacją eliptyczną.
    /// </summary>
    public double BDifference { get; set; }

    // ── Półosie elipsy natężenia pola magnetycznego ──────────────────────────

    /// <summary>
    /// Dłuższa półoś RMS elipsy natężenia pola magnetycznego [A/m].
    /// Ha = Ba / μ₀.
    /// </summary>
    public double Ha { get; set; }

    /// <summary>
    /// Krótsza półoś RMS elipsy natężenia pola magnetycznego [A/m].
    /// Hb = Bb / μ₀.
    /// </summary>
    public double Hb { get; set; }

    /// <summary>
    /// Diagnostyczna wartość weryfikacyjna odtworzona z półosi elipsy natężenia pola magnetycznego:
    /// HFromAxes = √(Ha² + Hb²) [A/m].
    /// Używana wyłącznie do sprawdzenia spójności między całkowitym natężeniem pola magnetycznego RMS
    /// a jego reprezentacją eliptyczną.
    /// </summary>
    public double HFromAxes { get; set; }

    /// <summary>
    /// Diagnostyczna różnica weryfikacyjna dla natężenia pola magnetycznego:
    /// HDifference = |H − HFromAxes| [A/m].
    /// Używana wyłącznie do sprawdzenia spójności między całkowitym natężeniem pola magnetycznego RMS
    /// a jego reprezentacją eliptyczną.
    /// </summary>
    public double HDifference { get; set; }
}

