namespace Top2000.Apps.CLI.Commands.Export;

public class ExportCommands : ICommand
{
    private readonly IEnumerable<ICommand<ExportCommands>> _subCommands;

    public ExportCommands(IEnumerable<ICommand<ExportCommands>> subCommands)
    {
        _subCommands = subCommands;
    }

    public Command Create()
    {
        var command = new Command("export", "Export data to various formats");
        
        foreach (var subCommand in _subCommands)
        {
            command.Subcommands.Add(subCommand.Create());
        }

        return command;
    }
}
