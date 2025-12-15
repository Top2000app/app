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
    
    /*

    private static Command CreateSongsCommand(ShowCommandHandler handler)
    {
        var songsCommand = new Command("songs", "Show songs from the Top 2000");
        
        var limitOption = new Option<int>(
            name: "--limit",
            description: "Number of items to show",
            getDefaultValue: () => 10);
        limitOption.AddAlias("-l");
        
        var yearOption = new Option<int?>(
            name: "--year",
            description: "Filter by specific year");
        yearOption.AddAlias("-y");
        
        songsCommand.AddOption(limitOption);
        songsCommand.AddOption(yearOption);
        
        songsCommand.SetHandler(async (int limit, int? year) =>
        {
            await handler.HandleShowSongsAsync(limit, year);
        }, limitOption, yearOption);
        
        return songsCommand;
    }

    private static Command CreateArtistsCommand(ShowCommandHandler handler)
    {
        var artistsCommand = new Command("artists", "Show artists from the Top 2000");
        
        var limitOption = new Option<int>(
            name: "--limit",
            description: "Number of items to show",
            getDefaultValue: () => 10);
        limitOption.AddAlias("-l");
        
        artistsCommand.AddOption(limitOption);
        
        artistsCommand.SetHandler(async (int limit) =>
        {
            await handler.HandleShowArtistsAsync(limit);
        }, limitOption);
        
        return artistsCommand;
    }

    private static Command CreateYearsCommand(ShowCommandHandler handler)
    {
        var yearsCommand = new Command("years", "Show data by year");
        
        yearsCommand.SetHandler(async () =>
        {
            await handler.HandleShowYearsAsync();
        });
        
        return yearsCommand;
    }
    */
   
}
