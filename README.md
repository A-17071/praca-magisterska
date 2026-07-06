# paca-magisterska
# Program do obliczania pola elektromagnetycznego

Aplikacja została przygotowana w technologii C#/.NET oraz Blazor. Program umożliwia obliczanie wartości natężenia pola elektrycznego i magnetycznego w otoczeniu napowietrznych linii elektroenergetycznych na podstawie zadanych parametrów geometrycznych i elektrycznych przewodów.

## Publikacja programu

Przed uruchomieniem wersji wykonywalnej należy opublikować projekt FrontEnd dla systemu Windows. W tym celu należy otworzyć terminal w głównym folderze rozwiązania i wykonać polecenie:

```powershell
dotnet publish .\FrontEnd\FrontEnd.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o .\publish\win-x64
```

Po wykonaniu tego polecenia zostanie utworzony folder publikacji:

```text
publish\win-x64
```


## Uruchomienie programu

Wersja wykonywalna programu znajduje się w folderze publikacji:

`publish`

lub, w zależności od użytej komendy, publikacji:

`publish\win-x64`

W folderze tym należy odnaleźć plik:

`FrontEnd.exe`

Aby uruchomić program, należy dwukrotnie kliknąć plik `FrontEnd.exe`. Po uruchomieniu aplikacja działa lokalnie jako aplikacja webowa i otwiera się w przeglądarce internetowej lub jest dostępna pod adresem lokalnym, np.:

`http://localhost:5000`

## Ważna uwaga

Nie należy przenosić samego pliku FrontEnd.exe poza folder publikacji. Aplikacja Blazor wymaga również dodatkowych plików i katalogów znajdujących się w tym samym folderze, takich jak wwwroot, pliki konfiguracyjne oraz pliki zasobów statycznych. Z tego powodu do poprawnego działania programu należy zachować całą zawartość folderu publish.

## Kod źródłowy

Kod źródłowy rozwiązania składa się z projektu interfejsu użytkownika FrontEnd oraz modułu obliczeniowego ModulKalkulacyjny. Projekt FrontEnd korzysta z modułu obliczeniowego poprzez referencję projektową.
