namespace ModulKalkulacyjny.Domain;

/// <summary>
/// Type of conductor bundle.
/// b_k denotes the number of sub-conductors in bundle k.
/// </summary>
public enum BundleType
{
    /// <summary>Single conductor – b_k = 1.</summary>
    Single,

    /// <summary>Two sub-conductors in a line – b_k = 2.</summary>
    Twin,

    /// <summary>Three sub-conductors at the corners of an equilateral triangle – b_k = 3.</summary>
    TripleTriangle,

    /// <summary>Four sub-conductors at the corners of a square – b_k = 4.</summary>
    QuadSquare
}

