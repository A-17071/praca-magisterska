namespace ModulKalkulacyjny.Domain;

/// <summary>
/// Typ wiązki przewodów.
/// b_k oznacza liczbę podprzewodników w wiązce k.
/// </summary>
public enum BundleType
{
    /// <summary>Pojedynczy przewód – b_k = 1.</summary>
    Single,

    /// <summary>Dwa podprzewodniki w linii – b_k = 2.</summary>
    Twin,

    /// <summary>Trzy podprzewodniki w narożnikach trójkąta równobocznego – b_k = 3.</summary>
    TripleTriangle,

    /// <summary>Cztery podprzewodniki w narożnikach kwadratu – b_k = 4.</summary>
    QuadSquare
}

