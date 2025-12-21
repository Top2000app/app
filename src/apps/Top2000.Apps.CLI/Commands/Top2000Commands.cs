using Top2000.Features;

namespace Top2000.Apps.CLI.Commands;

public class Top2000Command : RootCommand
{
    private readonly Top2000Services _top2000Services;
    private readonly IEnumerable<ICommand> _commands;

    public Top2000Command(Top2000Services top2000Services, IEnumerable<ICommand> commands) 
        : base("Top 2000 CLI Application")
    {
        _top2000Services = top2000Services;
        _commands = commands;
    }
    
    public async Task RunAsync(string[] args)
    {
        await InitialiseDatabaseAsync();
        
        foreach (var command in _commands)
        {
            Add(command.Create());
        }

        var result = Parse(args);
        
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