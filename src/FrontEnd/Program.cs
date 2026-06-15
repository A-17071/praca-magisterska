using FrontEnd.Components;
using ModulKalkulacyjny;
using ModulKalkulacyjny.Solvers;
using Radzen;

var builder = WebApplication.CreateBuilder(args);

// Dodaj komponenty Razor z interaktywnym renderowaniem po stronie serwera.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Zarejestruj kalkulator pola elektromagnetycznego jako singleton (jest bezstanowy).
builder.Services.AddSingleton<ElectromagneticFieldCalculator>();

// Zarejestruj usługi komponentów Radzen (wymagane dla wykresów i innych komponentów Radzen).
builder.Services.AddRadzenComponents();

var app = builder.Build();

// Konfiguracja potoku przetwarzania żądań HTTP.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // Domyślna wartość HSTS wynosi 30 dni. W środowisku produkcyjnym można ją zmienić – zob. https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

// UseStaticFiles zapewnia obsługę plików statycznych bez fingerprintingu jako fallback,
// gdy manifest MapStaticAssets nie został jeszcze wygenerowany (np. na świeżo sklonowanym projekcie).
app.UseStaticFiles();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();