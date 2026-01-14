using Top2000.Features;

namespace Top2000.Apps.CLI.Commands.Stats;

public class StatsCommand : ICommand
{
    private readonly IEnumerable<ICommand<StatsCommand>> _subCommands;

    public StatsCommand(IEnumerable<ICommand<StatsCommand>> subCommands)
    {
        _subCommands = subCommands;
    }
    
    public Command Create()
    {
        var command = new Command("stats", "Shows statistics about the Top2000");
        
        foreach (var subCommand in _subCommands)
        {
            command.Subcommands.Add(subCommand.Create());
        }

        return command;
    }
}