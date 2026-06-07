```mermaid

flowchart TD
A([Start]) --> B[Wprowadzenie danych geometrycznych linii]
B --> C[Wprowadzenie danych elektrycznych]
C --> D[Definicja punktów obserwacji]
D --> E[Sprawdzenie poprawności danych wejściowych]

    E --> F{Czy dane są poprawne?}
    F -- Nie --> G[Wyświetlenie komunikatu błędu]
    G --> B

    F -- Tak --> H[Utworzenie modelu obliczeniowego linii]

    H --> I{Wybór rodzaju obliczeń}

    I -- Pole elektryczne --> J[Utworzenie macierzy współczynników potencjałowych]
    J --> K[Wyznaczenie liniowych gęstości ładunku]
    K --> L[Obliczenie składowych pola elektrycznego Ex i Ey]
    L --> M[Obliczenie wartości wypadkowej E]

    I -- Pole magnetyczne --> N[Obliczenie odległości punktów obserwacji od przewodów]
    N --> O[Obliczenie składowych pola magnetycznego Hx i Hy]
    O --> P[Obliczenie wartości wypadkowej H]
    P --> Q[Opcjonalne obliczenie indukcji magnetycznej B]

    M --> R[Wyznaczenie wartości maksymalnych i minimalnych]
    Q --> R

    R --> S{Czy analiza czasowa jest włączona?}
    S -- Tak --> T[Obliczenie EDEPE lub EDEPM]
    S -- Nie --> U[Przygotowanie wyników końcowych]
    T --> U

    U --> V[Generowanie wykresów, map i tabel wyników]
    V --> W[Zapis lub eksport wyników]
    W --> X([Koniec])