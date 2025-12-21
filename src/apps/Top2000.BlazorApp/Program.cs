using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Top2000.BlazorApp;
using MudBlazor.Services;
using Top2000.Features;
using Top2000.Features.Json;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services
    .AddMudServices()
    .AddSingleton(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) })
    .AddTop2000Features<JsonFeatureAdapter>(configure =>
    {
        configure.ConfigureDataLoader(services =>
        {
            services.AddTransient<IDataLoader, Top2000DataLoader>();
        });
    })
    ;

var app = builder.Build();

// Initialize TrackService before running the app
var trackService = app.Services.GetRequiredService<Top2000Services>();
await trackService.InitialiseDataAsync();

await app.RunAsync();
