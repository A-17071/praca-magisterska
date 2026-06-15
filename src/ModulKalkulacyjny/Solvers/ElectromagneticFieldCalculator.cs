using System.Numerics;
using ModulKalkulacyjny.Domain;
using ModulKalkulacyjny.Helpers;

namespace ModulKalkulacyjny.Solvers;

/// <summary>
/// Główne publiczne API silnika obliczeniowego pola elektromagnetycznego.
/// Jest to jedyny punkt wejściowy wywoływany przez front-end Blazor.
///
/// Klasa jest bezstanowa – wszystkie dane wejściowe są przekazywane jako parametry metod,
/// dzięki czemu można ją bezpiecznie rejestrować jako usługę singleton lub scoped.
/// </summary>
public sealed class ElectromagneticFieldCalculator
{
    private readonly PotentialMatrixBuilder _matrixBuilder = new();
    private readonly ElectricFieldSolver    _eSolver       = new();
    private readonly MagneticFieldSolver    _bSolver       = new();

    // ── Metoda pomocnicza ────────────────────────────────────────────────────

    /// <summary>
    /// Tworzy zespolony fasor RMS z wartości skutecznej i kąta fazowego.
    ///   phasor = rmsValue · e^(j · angleDegrees · π/180)
    /// </summary>
    public static Complex Phasor(double rmsValue, double angleDegrees)
    {
        double rad = angleDegrees * Math.PI / 180.0;
        return Complex.FromPolarCoordinates(rmsValue, rad);
    }

    // ── Obliczenia pola (jeden stan pracy) ───────────────────────────────────

    /// <summary>
    /// Oblicza pole elektryczne i magnetyczne w każdym punkcie obserwacyjnym
    /// przy użyciu napięć i prądów przewodów w bieżącym stanie (jeden stan pracy).
    ///
    /// Algorytm:
    ///   1. Zbuduj macierz potencjałów P.
    ///   2. Rozwiąż P · q = U  dla liniowych gęstości ładunku q.
    ///   3. Dla każdego punktu: oblicz Ex, Ey, E  oraz  Bx, By, B, Hx, Hy, H.
    /// </summary>
    public IReadOnlyList<FieldResult> CalculateField(
        IReadOnlyList<Conductor> conductors,
        IReadOnlyList<ObservationPoint> points)
    {
        InputValidator.ValidateConductors(conductors);
        InputValidator.ValidateObservationPoints(points, conductors);

        var eConductors = conductors.Where(c => c.IncludeInElectricField).ToList();
        var potMatrix   = _matrixBuilder.Build(conductors);
        var voltages    = eConductors.Select(c => c.Voltage).ToArray();
        var q           = _eSolver.SolveLineCharges(potMatrix, voltages);

        var results = new List<FieldResult>(points.Count);

        foreach (var point in points)
        {
            var (ex, ey, e, ea, eb)              = _eSolver.CalculateAtPoint(eConductors, q, point);
            var (bx, by, b, hx, hy, h, ba, bb, ha, hb) = _bSolver.CalculateAtPoint(conductors, point);
            double eFromAxes = Math.Sqrt(ea * ea + eb * eb);
            double eDifference = Math.Abs(e - eFromAxes);
            double bFromAxes = Math.Sqrt(ba * ba + bb * bb);
            double bDifference = Math.Abs(b - bFromAxes);
            double hFromAxes = Math.Sqrt(ha * ha + hb * hb);
            double hDifference = Math.Abs(h - hFromAxes);

            results.Add(new FieldResult
            {
                Point = point,
                Ex = ex, Ey = ey, E = e, Ea = ea, Eb = eb,
                Bx = bx, By = by, B = b, Ba = ba, Bb = bb,
                Hx = hx, Hy = hy, H = h, Ha = ha, Hb = hb,
                EFromAxes = eFromAxes, EDifference = eDifference,
                BFromAxes = bFromAxes, BDifference = bDifference,
                HFromAxes = hFromAxes, HDifference = hDifference
            });
        }

        return results;
    }

    // ── Obliczenia ekspozycji (wiele stanów pracy) ───────────────────────────

    /// <summary>
    /// Oblicza dawki ekspozycji i statystyki w każdym punkcie obserwacyjnym
    /// przez iterację po wszystkich stanach pracy.
    ///
    /// Dla każdego stanu j o czasie trwania Δtj:
    ///   EDEP += K^p · Δtj   (energetyczna dawka ekspozycji, całka dyskretna)
    ///
    /// Po zakończeniu pętli:
    ///   KAverage    = (Σ K · Δt) / T
    ///   KEquivalent = (EDEP / T)^(1/p)    (równoważne stałe pole)
    /// </summary>
    public IReadOnlyList<ExposureResult> CalculateExposure(
        IReadOnlyList<Conductor> conductors,
        IReadOnlyList<ObservationPoint> points,
        IReadOnlyList<OperatingState> states,
        double exponentP)
    {
        InputValidator.ValidateConductors(conductors);
        InputValidator.ValidateObservationPoints(points, conductors);
        InputValidator.ValidateOperatingStates(states, conductors.Count);
        InputValidator.ValidateExponent(exponentP);

        int n = points.Count;

        // Akumulatory
        double[] edepe  = new double[n];
        double[] edepmB = new double[n];
        double[] edepmH = new double[n];
        double[] eMax   = Enumerable.Repeat(double.MinValue, n).ToArray();
        double[] eMin   = Enumerable.Repeat(double.MaxValue, n).ToArray();
        double[] bMax   = Enumerable.Repeat(double.MinValue, n).ToArray();
        double[] bMin   = Enumerable.Repeat(double.MaxValue, n).ToArray();
        double[] hMax   = Enumerable.Repeat(double.MinValue, n).ToArray();
        double[] hMin   = Enumerable.Repeat(double.MaxValue, n).ToArray();
        double[] sumE   = new double[n];
        double[] sumB   = new double[n];
        double[] sumH   = new double[n];

        double totalTime = states.Sum(s => s.DurationSeconds);

        // Utwórz modyfikowalną kopię przewodów do zastosowania napięć/prądów dla każdego stanu
        var cCopy = conductors.Select(c => new Conductor
        {
            Id = c.Id, X = c.X, Y = c.Y, Radius = c.Radius,
            IncludeInElectricField = c.IncludeInElectricField,
            IncludeInMagneticField = c.IncludeInMagneticField
        }).ToList();

        foreach (var state in states)
        {
            double dt = state.DurationSeconds;

            // Zastosuj napięcia i prądy dla bieżącego stanu
            for (int i = 0; i < cCopy.Count; i++)
            {
                cCopy[i].Voltage = state.Voltages[i];
                cCopy[i].Current = state.Currents[i];
            }

            var eConductors = cCopy.Where(c => c.IncludeInElectricField).ToList();
            var potMatrix   = _matrixBuilder.Build(cCopy);
            var voltages    = eConductors.Select(c => c.Voltage).ToArray();
            var q           = _eSolver.SolveLineCharges(potMatrix, voltages);

            for (int i = 0; i < n; i++)
            {
                var point = points[i];
                var (_, _, e, _, _)                         = _eSolver.CalculateAtPoint(eConductors, q, point);
                var (_, _, b, _, _, h, _, _, _, _)          = _bSolver.CalculateAtPoint(cCopy, point);

                // EDEP = Σ K^p · Δt
                edepe[i]  += Math.Pow(e, exponentP) * dt;
                edepmB[i] += Math.Pow(b, exponentP) * dt;
                edepmH[i] += Math.Pow(h, exponentP) * dt;

                if (e > eMax[i]) eMax[i] = e;
                if (e < eMin[i]) eMin[i] = e;
                if (b > bMax[i]) bMax[i] = b;
                if (b < bMin[i]) bMin[i] = b;
                if (h > hMax[i]) hMax[i] = h;
                if (h < hMin[i]) hMin[i] = h;

                // Sumy ważone do obliczenia średnich w czasie
                sumE[i] += e * dt;
                sumB[i] += b * dt;
                sumH[i] += h * dt;
            }
        }

        var results = new List<ExposureResult>(n);
        for (int i = 0; i < n; i++)
        {
            results.Add(new ExposureResult
            {
                Point       = points[i],
                Edepe       = edepe[i],
                EdepmB      = edepmB[i],
                EdepmH      = edepmH[i],
                EMax        = eMax[i] > double.MinValue ? eMax[i] : 0,
                EMin        = eMin[i] < double.MaxValue ? eMin[i] : 0,
                BMax        = bMax[i] > double.MinValue ? bMax[i] : 0,
                BMin        = bMin[i] < double.MaxValue ? bMin[i] : 0,
                HMax        = hMax[i] > double.MinValue ? hMax[i] : 0,
                HMin        = hMin[i] < double.MaxValue ? hMin[i] : 0,
                EAverage    = totalTime > 0 ? sumE[i] / totalTime : 0,
                BAverage    = totalTime > 0 ? sumB[i] / totalTime : 0,
                HAverage    = totalTime > 0 ? sumH[i] / totalTime : 0,
                // Równoważne stałe pole: Keq = (EDEP / T)^(1/p)
                EEquivalent = totalTime > 0 ? Math.Pow(edepe[i]  / totalTime, 1.0 / exponentP) : 0,
                BEquivalent = totalTime > 0 ? Math.Pow(edepmB[i] / totalTime, 1.0 / exponentP) : 0,
                HEquivalent = totalTime > 0 ? Math.Pow(edepmH[i] / totalTime, 1.0 / exponentP) : 0,
            });
        }

        return results;
    }
}



