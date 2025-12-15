using System.CommandLine;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Top2000.Apps.CLI.Commands.Show;
using Top2000.Apps.CLI.Commands.Export;
using Top2000.Apps.CLI.Commands;
using Top2000.Apps.CLI.Commands.Export.csv;
using Top2000.Apps.CLI.Commands.Export.json;
using Top2000.Apps.CLI.Commands.Search;
using Top2000.Apps.CLI.Database;
using Top2000.Data.ClientDatabase;
using Top2000.Features.SQLite;

var host = Host.CreateApplicationBuilder(args);

host.Services
    .AddTop2000Features()
    .AddDbContext<Top2000DbContext>()

    .AddSingleton<ExportCsvCommandHandler>()
    .AddSingleton<ExportJsonCommandHandler>()
    .AddSingleton<ICommand, ExportCommands>()

    .AddSingleton<ShowCommandHandler>()
    .AddSingleton<ICommand, ShowCommands>()

    .AddSingleton<SearchCommandHandler>()
    .AddSingleton<ICommand, SearchCommand>()


    ;
var app = host.Build();
var databaseGen = app.Services.GetRequiredService<IUpdateClientDatabase>();
var top2000 = app.Services.GetRequiredService<Top2000AssemblyDataSource>();

await AnsiConsole.Status()
    .StartAsync("Initialising database...", async ctx =>
    {
        ctx.Spinner(Spinner.Known.Dots);
        ctx.SpinnerStyle(Style.Parse("green"));
        
        await databaseGen.RunAsync(top2000);
        
        ctx.Status("Database ready!");
        await Task.Delay(500); // Brief pause to show completion
    });


var commands = app.Services.GetRequiredService<IEnumerable<ICommand>>(); 

var rootCommand = new RootCommand("Top 2000 CLI Application");

foreach (var command in commands)
{
    rootCommand.Add(command.Create());
}

var result = rootCommand.Parse(args);
await result.InvokeAsync();