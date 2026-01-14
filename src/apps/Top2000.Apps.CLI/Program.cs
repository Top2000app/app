using DownloaderApp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Top2000.Apps.CLI.Commands.Show;
using Top2000.Apps.CLI.Commands.Export;
using Top2000.Apps.CLI.Commands;
using Top2000.Apps.CLI.Commands.Export.Isam;
using Top2000.Apps.CLI.Commands.Info;
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
    .AddRootCommand<Top2000Command>()
    ;

host.Services
    .AddSingleton<Database>()
    .AddCommand<ExportCommands>()
    .AddCommand<ExportCommands, ExportJsonCommand>()
    .AddCommand<ExportCommands, ExportApiCommand>()
    .AddCommand<ExportCommands, ExportCsvCommand>()
  //  .AddSubCommand<ExportIsamCommand>()
    ;

host.Services
    .AddSingleton<ShowListingCommand>()
    .AddCommand<ShowCommands>()
    .AddCommand<ShowCommands, ShowNowCommand>()
    .AddCommand<ShowCommands, ShowEditionsCommand>()
    .AddCommand<ShowCommands, ShowListingCommand>()
    ;
    
host.Services
    .AddCommand<SearchCommand>() 
    ;

host.Services
    .AddCommand<StatsCommand>()
    .AddCommand<StatsCommand, StatsListingCommand>()
    ;

host.Services.AddCommand<InfoCommand>()
    ;

var app = host.Build();

await app.Services
    .GetRequiredService<Top2000Command>()
    .RunAsync(args);