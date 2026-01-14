using Microsoft.Extensions.DependencyInjection;
using Top2000.Data.ClientDatabase;
using Top2000.Features;

namespace Top2000.Apps.CLI.Commands;

public class Top2000Command : RootCommand
{
    private readonly Top2000Services _top2000Services;
    private readonly IServiceProvider _serviceProvider;
    private readonly Top2000ServiceBuilder _builder;

    public Top2000Command(Top2000Services top2000Services, IServiceProvider serviceProvider, Top2000ServiceBuilder builder) 
        : base("Top 2000 CLI Application. A command line interface for interacting with the Top 2000 database.")
    {
        _top2000Services = top2000Services;
        _serviceProvider = serviceProvider;
        _builder = builder;

        base.Add(new Option<bool>("--skip-db-init")
        {
            Description =  "Skip database initialisation on startup",
            DefaultValueFactory = (_) => false
        });
        
        base.Add(new Option<bool>("--reset-db")
        {
            Description = "Deletes and reinitialise the database on startup",
            DefaultValueFactory = (_) => false
        });
    }

    public async Task RunAsync(string[] args)
    {
        var subCommands = _serviceProvider.GetKeyedServices<CommandBase>(typeof(Top2000Command));
        foreach (var command in subCommands)
        {
            Add(command.Create(_serviceProvider));
        }

        var result = Parse(args);
        
        var dbLocation = Path.Combine(_builder.Directory, _builder.Name);
        if (result.GetRequiredValue<bool>("--reset-db"))
        {
            File.Delete(dbLocation);
        }

        if (result.Errors.Count == 0)
        {
            if (result.GetRequiredValue<bool>("--skip-db-init"))
            {
                if (!File.Exists(dbLocation))
                {
                    AnsiConsole.MarkupLine("[yellow]Database file not found, cannot skip initialisation.[/]");
                    await InitialiseDatabaseAsync();
                }
            }
            else
            {
                await InitialiseDatabaseAsync();
            }
        }
            
        await result.InvokeAsync();
    }

    private Task InitialiseDatabaseAsync()
    {
        return AnsiConsole.Status()
            .StartAsync("Initialising database...", async ctx =>
            {
                ctx.Spinner(Spinner.Known.Dots);
                ctx.SpinnerStyle(Style.Parse("green"));

                await _top2000Services.InitialiseDataAsync();
                await _top2000Services.UpdateAsync();
            
                ctx.Status("Database ready!");
                await Task.Delay(500);
            });
    }
}