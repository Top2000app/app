using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Top2000.Apps.CLI.Commands.Show;
using Top2000.Apps.CLI.Commands.Export;
using Top2000.Apps.CLI.Commands;
using Top2000.Apps.CLI.Commands.Search;
using Top2000.Apps.CLI.Database;
using Top2000.Features;
using Top2000.Features.SQLite;

var host = Host.CreateApplicationBuilder(args);

host.Logging.ClearProviders();

host.Services
    .AddTop2000Features<SqliteFeatureAdapter>()
    .AddDbContext<Top2000DbContext>()
    .AddSingleton<Top2000Command>()
    ;

host.Services
    .AddCommand<ExportCommands>()
    .AddSubCommand<ExportJsonCommand>()
    .AddSubCommand<ExportApiCommand>()
    .AddSubCommand<ExportCsvCommand>()
    ;

host.Services
    .AddSingleton<ShowListingCommand>()
    .AddCommand<ShowCommands>()
    .AddSubCommand<ShowNowCommand>()
    .AddSubCommand<ShowEditionsCommand>()
    .AddSubCommand<ShowListingCommand>()
    ;
    
host.Services
    .AddSingleton<SearchCommandHandler>()
    .AddSingleton<ICommand, SearchCommand>() 
    ;

var app = host.Build();

await app.Services
    .GetRequiredService<Top2000Command>()
    .RunAsync(args);