namespace Top2000.Apps.CLI.Commands.Search;

public class SearchCommand : ICommand
{
    private readonly SearchCommandHandler _handler;

    public SearchCommand(SearchCommandHandler handler)
    {
        _handler = handler;
    }
    
    public Command Create()
    {
        var showCommand = new Command("search", "Display items from the database")
        {
            new Option<string>("--query", "-q")
            {
                Description = "Search query string"
            },
            new Option<bool>("--showIds", "-i")
            {
                Description = "Whether to show IDs in the results",
            }
        };

        showCommand.SetAction(_handler.HandleSearchAsync);
        
        return showCommand;
    }
}