namespace Top2000.Apps.CLI.Commands.Show;

public class ShowCommands : ICommand
{
    private readonly IEnumerable<ICommand<ShowCommands>> _subCommands;

    public ShowCommands(IEnumerable<ICommand<ShowCommands>> subCommands)
    {
        _subCommands = subCommands;
    }

    public Command Create()
    {
        var command = new Command("show", "Display items from the database");

        foreach (var subCommand in _subCommands)
        {
            command.Subcommands.Add(subCommand.Create());
        }

        return command;
    }
}
