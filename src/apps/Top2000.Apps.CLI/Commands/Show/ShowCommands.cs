namespace Top2000.Apps.CLI.Commands.Show;

public class ShowCommands : ICommand
{
    private readonly ShowCommandHandler _handler;

    public ShowCommands(ShowCommandHandler handler)
    {
        _handler = handler;
    }
    
    public Command Create()
    {
        var showCommand = new Command("show", "Display items from the database")
        {
            ShowEditionsCommand(),
            ShowEditionCommand()
        };

        return showCommand;
    }
    
    private Command ShowEditionsCommand()
    {
        var editionsCommand = new Command("editions", "Show Top 2000 editions");
        
        editionsCommand.SetAction(_handler.HandleShowEditionsAsync);
        
        return editionsCommand;
    }

    private Command ShowEditionCommand()
    {
        var editionCommand = new Command("edition", "Show a specific Top 2000 edition");
        
        editionCommand.SetAction(_handler.HandleShowEditionAsync);
        
        editionCommand.Add(new Option<int>("--year", "-y")
        {
            Description = "Year of the edition to show",
        });
        
        return editionCommand;
    }
}
