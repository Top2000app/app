using DownloaderApp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Top2000.Apps.CLI.Commands.Show;
using Top2000.Apps.CLI.Commands.Export;
using Top2000.Apps.CLI.Commands;
using Top2000.Apps.CLI.Commands.Export.Isam;
using Top2000.Apps.CLI.Commands.Search;
using Top2000.Apps.CLI.Commands.Stats;
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
    .AddSingleton<Database>()
    .AddCommand<ExportCommands>()
    .AddSubCommand<ExportJsonCommand>()
    .AddSubCommand<ExportApiCommand>()
    .AddSubCommand<ExportCsvCommand>()
    .AddSubCommand<ExportIsamCommand>()
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

host.Services
    .AddCommand<StatsCommand>()
    .AddSubCommand<StatsListingCommand>()
    ;

var app = host.Build();

await app.Services
    .GetRequiredService<Top2000Command>()
    .RunAsync(args);