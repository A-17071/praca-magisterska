using FrontEnd.Components;
using ModulKalkulacyjny;
using Radzen;

var builder = WebApplication.CreateBuilder(args);

// Add Razor components with interactive server rendering.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Register the electromagnetic field calculator as a singleton (it is stateless).
builder.Services.AddSingleton<ElectromagneticFieldCalculator>();

// Register Radzen component services (required for charts and other Radzen components).
builder.Services.AddRadzenComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();