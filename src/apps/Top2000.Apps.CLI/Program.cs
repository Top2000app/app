using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Top2000.Apps.CLI.Commands.Show;
using Top2000.Apps.CLI.Commands.Export;
using Top2000.Apps.CLI.Commands;
using Top2000.Apps.CLI.Commands.Export.Api;
using Top2000.Apps.CLI.Commands.Export.Csv;
using Top2000.Apps.CLI.Commands.Export.Json;
using Top2000.Apps.CLI.Commands.Search;
using Top2000.Apps.CLI.Database;
using Top2000.Features;
using Top2000.Features.SQLite;

var host = Host.CreateApplicationBuilder(args);

host.Services
    .AddTop2000Features<SqliteFeatureAdapter>()
    .AddDbContext<Top2000DbContext>()
    .AddSingleton<Top2000Command>()

    .AddSingleton<ExportApiCommandHandler>()
    .AddSingleton<ExportCsvCommandHandler>()
    .AddSingleton<ExportJsonCommandHandler>()
    .AddSingleton<ICommand, ExportCommands>()

    .AddSingleton<ShowCommandHandler>()
    .AddSingleton<ICommand, ShowCommands>()

    .AddSingleton<SearchCommandHandler>()
    .AddSingleton<ICommand, SearchCommand>() 
    ;

var app = host.Build();

await app.Services
    .GetRequiredService<Top2000Command>()
    .RunAsync(args);