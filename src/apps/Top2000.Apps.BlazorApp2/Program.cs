using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Top2000.Apps.BlazorApp2;
using Top2000.Apps.MudBlazorApp;
using Top2000.Features.Json;
using Top2000.Features;
using Top2000.Features.Json.Data;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services
    .AddMudServices()
    .AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) })
    .AddTop2000Features<JsonFeatureAdapter>(configure =>
    {
        configure.ConfigureDataLoader(services =>
        {
            services.AddTransient<IDataLoader, Top2000DataLoader>();
        });
    });

var app = builder.Build();

await app.Services
    .GetRequiredService<JsonPartialDataInitialiser>()
    .InitialiseVersionDataAsync();

await app.RunAsync();