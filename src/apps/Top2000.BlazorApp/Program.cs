using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Top2000.BlazorApp;
using Top2000.BlazorApp.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<EditionService>();
builder.Services.AddSingleton<TrackService>();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

var app = builder.Build();

// Initialize TrackService before running the app
var trackService = app.Services.GetRequiredService<TrackService>();
await trackService.InitializeAsync();

await app.RunAsync();
