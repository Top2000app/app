using Top2000.Features;
using Top2000.Features.Listings;

namespace Top2000.Apps.CLI.Commands.Show;

public class ShowListingCommand : ICommand<ShowCommands>
{
    private readonly Top2000Services _top2000Services;

    public ShowListingCommand(Top2000Services top2000Services)
    {
        _top2000Services = top2000Services;
    }
    
    
    public Command Create()
    {
        var editionCommand = new Command("edition", "Show a specific Top 2000 edition");
         
        editionCommand.SetAction(HandleShowEditionAsync);
         
        editionCommand.Add(new Option<int>("--year", "-y")
        {
            Description = "Year of the edition to show",
        });
         
        return editionCommand;
    }

    public async Task<int> HandleShowEditionAsync(ParseResult result, CancellationToken token)
    {
        var year = result.GetValue<int?>("--year");
        
        // If no year was specified, ask the user to select one
        if (!year.HasValue)
        {
            var availableEditions = (await _top2000Services.AllEditionsAsync(token))
                .Select(e => e.Year)
                .ToList();

            if (availableEditions.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No editions found in the database.[/]");
                return 0;
            }

            year = AnsiConsole.Prompt(
                new SelectionPrompt<int>()
                    .Title("[green]Select a year to view:[/]")
                    .AddChoices(availableEditions)
                    .UseConverter(y => y.ToString()));
        }

        var listingsForYear = await _top2000Services.AllListingsOfEditionAsync(year.Value, token);

        if (listingsForYear.Count == 0)
        {
            AnsiConsole.MarkupLine($"[yellow]No listings found for year {year.Value}.[/]");
            return 0;
        }

        var listings = listingsForYear.ToList();
        
        // Ask user how they want to view the data
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title($"[green]Found {listings.Count} songs for {year.Value}. How would you like to view them?[/]")
                .AddChoices("Show all at once", "Show paginated (25 per page)", "Show top 100 only", "Show summary statistics"));

        switch (choice)
        {
            case "Show all at once":
                TrackListView.DisplayTable(listings, $"Top 2000 - {year.Value} (All {listings.Count} songs)");
                break;
                
            case "Show paginated (25 per page)":
                DisplayPaginated(listings, year.Value);
                break;
                
            case "Show top 100 only":
                var top100 = listings.Take(100).ToList();
                TrackListView.DisplayTable(top100, $"Top 2000 - {year.Value} (Top 100)");
                break;
                
            case "Show summary statistics":
                DisplaySummaryStats(listings, year.Value);
                break;
        }

        return 0;
    }

   

    private static void DisplayPaginated(List<TrackListing> listings, int year)
    {
        const int pageSize = 25;
        var totalPages = (int)Math.Ceiling(listings.Count / (double)pageSize);
        var currentPage = 1;

        while (true)
        {
            var startIndex = (currentPage - 1) * pageSize;
            var pageItems = listings.Skip(startIndex).Take(pageSize).ToList();
            
            AnsiConsole.Clear();
            TrackListView.DisplayTable(pageItems, $"Top 2000 - {year} (Page {currentPage}/{totalPages})");
            
            var options = new List<string>();
            if (currentPage < totalPages) options.Add("Next page");
            if (currentPage > 1) options.Add("Previous page");
            options.AddRange(["Jump to page", "Search in results", "Exit"]);

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Navigation:")
                    .AddChoices(options));

            switch (choice)
            {
                case "Previous page":
                    currentPage--;
                    break;
                case "Next page":
                    currentPage++;
                    break;
                case "Jump to page":
                    var targetPage = AnsiConsole.Prompt(
                        new TextPrompt<int>($"Enter page number (1-{totalPages}):")
                            .ValidationErrorMessage("[red]Invalid page number[/]")
                            .Validate(page => page >= 1 && page <= totalPages));
                    currentPage = targetPage;
                    break;
                
                case "Exit":
                    return;
            }
        }
    }


    private static void DisplaySummaryStats(List<TrackListing> listings, int year)
    {
        var newEntries = listings.Count(x => x.DeltaType == TrackListingDeltaType.New);
        var recurringEntries = listings.Count(x => x.DeltaType == TrackListingDeltaType.Recurring);
        var movedUp = listings.Count(x => x.DeltaType == TrackListingDeltaType.Increased);
        var movedDown = listings.Count(x => x.DeltaType == TrackListingDeltaType.Decreased);
        var stayedSame = listings.Count(x => x.DeltaType == TrackListingDeltaType.NoChange);

        var panel = new Panel(
            new Markup($"""
                [bold]Top 2000 {year} - Summary Statistics[/]
                
                [green]📊 Total Songs:[/] {listings.Count}
                
                [yellow]🆕 New Entries:[/] {newEntries}
                [yellow]↻ Returning Songs:[/] {recurringEntries}
                
                [green]📈 Moved Up:[/] {movedUp}
                [red]📉 Moved Down:[/] {movedDown}
                [dim]=  Stayed Same:[/] {stayedSame}
                
                [bold]Biggest Changes:[/]
                
                [green]📈 Highest Climbers:[/]
                {string.Join("\n", listings
                    .Where(l => l.DeltaType == TrackListingDeltaType.Increased)
                    .OrderByDescending(l => l.Delta)
                    .Take(5)
                    .Select((l, i) => $"{i + 1,2}. {l.Artist.EscapeMarkup()} - {l.Title.EscapeMarkup()} (↑{l.Delta})"))}
                
                [red]📉 Biggest Drops:[/]
                {string.Join("\n", listings
                    .Where(l => l.DeltaType == TrackListingDeltaType.Decreased)
                    .OrderBy(l => l.Delta)
                    .Take(5)
                    .Select((l, i) => $"{i + 1,2}. {l.Artist.EscapeMarkup()} - {l.Title.EscapeMarkup()} (↓{Math.Abs(l.Delta)})"))}
                """))
            .Header("Statistics")
            .Border(BoxBorder.Rounded)
            .BorderColor(Color.Blue);

        AnsiConsole.Write(panel);
    }


}
