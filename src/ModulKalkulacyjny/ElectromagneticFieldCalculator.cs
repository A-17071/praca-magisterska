using System.Numerics;

namespace ModulKalkulacyjny;

/// <summary>
/// Main public API for the electromagnetic field calculation engine.
/// This is the single entry point called by the Blazor front end.
///
/// The class is stateless – all inputs are passed as method parameters
/// so it is safe to register as a singleton or scoped service.
/// </summary>
public sealed class ElectromagneticFieldCalculator
{
    private readonly PotentialMatrixBuilder _matrixBuilder = new();
    private readonly ElectricFieldSolver    _eSolver       = new();
    private readonly MagneticFieldSolver    _bSolver       = new();

    // ── Helper ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Creates a complex RMS phasor from an RMS magnitude and a phase angle.
    ///   phasor = rmsValue · e^(j · angleDegrees · π/180)
    /// </summary>
    public static Complex Phasor(double rmsValue, double angleDegrees)
    {
        double rad = angleDegrees * Math.PI / 180.0;
        return Complex.FromPolarCoordinates(rmsValue, rad);
    }

    // ── Field calculation (single operating state) ───────────────────────────

    /// <summary>
    /// Calculates electric and magnetic field at every observation point using
    /// the conductor voltages and currents as they are (single operating state).
    ///
    /// Algorithm:
    ///   1. Build potential matrix P.
    ///   2. Solve P · q = U  for line charge densities q.
    ///   3. For each point: calculate Ex, Ey, E  and  Bx, By, B, Hx, Hy, H.
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
            var (ex, ey, e) = _eSolver.CalculateAtPoint(eConductors, q, point);
            var (bx, by, b, hx, hy, h) = _bSolver.CalculateAtPoint(conductors, point);

            results.Add(new FieldResult
            {
                Point = point,
                Ex = ex, Ey = ey, E = e,
                Bx = bx, By = by, B = b,
                Hx = hx, Hy = hy, H = h
            });
        }

        return results;
    }

    // ── Exposure calculation (multiple operating states) ─────────────────────

    /// <summary>
    /// Calculates exposure doses and statistics at every observation point
    /// by iterating over all operating states.
    ///
    /// For each state j with duration Δtj:
    ///   EDEP += K^p · Δtj   (energetic exposure dose, discrete integral)
    ///
    /// After the loop:
    ///   KAverage    = (Σ K · Δt) / T
    ///   KEquivalent = (EDEP / T)^(1/p)    (equivalent constant field)
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

        // Accumulators
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

        // Create a mutable copy of conductors to apply per-state voltages/currents
        var cCopy = conductors.Select(c => new Conductor
        {
            Id = c.Id, X = c.X, Y = c.Y, Radius = c.Radius,
            IncludeInElectricField = c.IncludeInElectricField,
            IncludeInMagneticField = c.IncludeInMagneticField
        }).ToList();

        foreach (var state in states)
        {
            double dt = state.DurationSeconds;

            // Apply this state's voltages and currents
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
                var (_, _, e) = _eSolver.CalculateAtPoint(eConductors, q, point);
                var (_, _, b, _, _, h) = _bSolver.CalculateAtPoint(cCopy, point);

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

                // Weighted sums for time-averaged values
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
                // Equivalent constant field: Keq = (EDEP / T)^(1/p)
                EEquivalent = totalTime > 0 ? Math.Pow(edepe[i]  / totalTime, 1.0 / exponentP) : 0,
                BEquivalent = totalTime > 0 ? Math.Pow(edepmB[i] / totalTime, 1.0 / exponentP) : 0,
                HEquivalent = totalTime > 0 ? Math.Pow(edepmH[i] / totalTime, 1.0 / exponentP) : 0,
            });
        }

        return results;
    }
}



