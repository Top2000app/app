using Microsoft.EntityFrameworkCore;
using Top2000.Apps.CLI.Database;
using Top2000.Features;
using Top2000.Features.Listing;

namespace Top2000.Apps.CLI.Commands.Show;

public class ShowCommandHandler
{
    private readonly Top2000DbContext _dbContext;
    private readonly Top2000Services _top2000Services;

    public ShowCommandHandler(Top2000DbContext dbContext, Top2000Services top2000Services)
    {
        _dbContext = dbContext;
        _top2000Services = top2000Services;
    }
    
    public async Task<int> HandleShowEditionsAsync(ParseResult result, CancellationToken token)
    {
        var editions = await _dbContext.Editions
            .OrderBy(e => e.Year)
            .ToListAsync(cancellationToken: token);

        if (editions.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No editions found.[/]");
            return 0;
        }

        var table = new Table();
        table.AddColumn("[bold]#[/]");
        table.AddColumn("[bold]Year[/]");
        table.AddColumn("[bold]Start Date[/]");
        table.AddColumn("[bold]End Date[/]");
        table.AddColumn("[bold]Duration[/]");
        table.AddColumn("[bold]Has Play Time[/]");

        var index = 1;
        foreach (var edition in editions)
        {
            var startLocal = edition.StartUtcDateAndTime.ToLocalTime();
            var endLocal = edition.EndUtcDateAndTime.ToLocalTime();
            
            // Calculate duration - for 2023 special edition, use 2024 duration plus 43.5 hours
            var duration = endLocal - startLocal;
            if (edition.Year == 2023)
            {
                // Find 2024 edition to use its duration as base
                var edition2024 = editions.FirstOrDefault(e => e.Year == 2024);
                if (edition2024 != null)
                {
                    var start2024Local = edition2024.StartUtcDateAndTime.ToLocalTime();
                    var end2024Local = edition2024.EndUtcDateAndTime.ToLocalTime();
                    var duration2024 = end2024Local - start2024Local;
                    duration = duration2024 + TimeSpan.FromHours(43.5);
                }
                else
                {
                    // Fallback if 2024 edition is not found - add 43.5 hours to actual 2023 duration
                    duration = duration + TimeSpan.FromHours(43.5);
                }
            }
            
            var durationText = $"{duration.Days}d {duration.Hours}h {duration.Minutes}m";
            var hasPlayTime = edition.HasPlayDateAndTime ? "[green]✓[/]" : "[red]✗[/]";

            table.AddRow(
                index.ToString(),
                edition.Year.ToString(),
                startLocal.ToString("yyyy-MM-dd HH:mm"),
                endLocal.ToString("yyyy-MM-dd HH:mm"),
                durationText,
                hasPlayTime
            );
            
            index++;
        }

        AnsiConsole.Write(table);
        
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine($"[dim]Found {editions.Count} edition(s)[/]");
        
        return 0;
    }

    public async Task<int> HandleShowEditionAsync(ParseResult result, CancellationToken token)
    {
        var year = result.GetValue<int?>("--year");
        
        // If no year was specified, ask the user to select one
        if (!year.HasValue)
        {
            var availableEditions = await _dbContext.Editions
                .OrderByDescending(e => e.Year)
                .Select(e => e.Year)
                .ToListAsync(cancellationToken: token);

            if (!availableEditions.Any())
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

        if (!listingsForYear.Any())
        {
            AnsiConsole.MarkupLine($"[yellow]No listings found for year {year.Value}.[/]");
            return 0;
        }

        var listings = listingsForYear.ToList();
        
        // Ask user how they want to view the data
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title($"[green]Found {listings.Count} songs for {year.Value}. How would you like to view them?[/]")
                .AddChoices(new[] {
                    "Show all at once",
                    "Show paginated (25 per page)",
                    "Show top 100 only",
                    "Show summary statistics"
                }));

        switch (choice)
        {
            case "Show all at once":
                DisplayTable(listings, $"Top 2000 - {year.Value} (All {listings.Count} songs)");
                break;
                
            case "Show paginated (25 per page)":
                DisplayPaginated(listings, year.Value);
                break;
                
            case "Show top 100 only":
                var top100 = listings.Take(100).ToList();
                DisplayTable(top100, $"Top 2000 - {year.Value} (Top 100)");
                break;
                
            case "Show summary statistics":
                DisplaySummaryStats(listings, year.Value);
                break;
        }

        return 0;
    }

    private void DisplayTable(List<TrackListing> listings, string title)
    {
        var table = new Table()
            .Title(title)
            .Border(TableBorder.Rounded)
            .BorderColor(Color.Grey);
            
        table.AddColumn(new TableColumn("[bold]#[/]").Centered());
        table.AddColumn(new TableColumn("[bold]Δ[/]").Centered());
        table.AddColumn("[bold]Title[/]");
        table.AddColumn("[bold]Artist[/]");

        foreach (var listing in listings)
        {
            string changeText = GetChangeText(listing);
            
            table.AddRow(
                listing.Position.ToString(),
                changeText,
                listing.Title.EscapeMarkup(),
                listing.Artist.EscapeMarkup()
            );
        }

        AnsiConsole.Write(table);
        AnsiConsole.MarkupLine($"[dim]Showing {listings.Count} song(s)[/]");
    }

    private void DisplayPaginated(List<TrackListing> listings, int year)
    {
        const int pageSize = 25;
        var totalPages = (int)Math.Ceiling(listings.Count / (double)pageSize);
        var currentPage = 1;

        while (true)
        {
            var startIndex = (currentPage - 1) * pageSize;
            var pageItems = listings.Skip(startIndex).Take(pageSize).ToList();
            
            AnsiConsole.Clear();
            DisplayTable(pageItems, $"Top 2000 - {year} (Page {currentPage}/{totalPages})");
            
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


    private void DisplaySummaryStats(List<TrackListing> listings, int year)
    {
        var newEntries = listings.Count(l => !l.Delta.HasValue && !l.IsRecurring);
        var recurringEntries = listings.Count(l => l.IsRecurring);
        var movedUp = listings.Count(l => l.Delta.HasValue && l.Delta > 0);
        var movedDown = listings.Count(l => l.Delta.HasValue && l.Delta < 0);
        var stayedSame = listings.Count(l => l.Delta.HasValue && l.Delta == 0);

        var panel = new Panel(
            new Markup($"""
                [bold]Top 2000 {year} - Summary Statistics[/]
                
                [green]📊 Total Songs:[/] {listings.Count}
                
                [yellow]🆕 New Entries:[/] {newEntries}
                [yellow]🔄 Returning Songs:[/] {recurringEntries}
                
                [green]📈 Moved Up:[/] {movedUp}
                [red]📉 Moved Down:[/] {movedDown}
                [dim]➡️  Stayed Same:[/] {stayedSame}
                
                [bold]Biggest Changes:[/]
                
                [green]📈 Highest Climbers:[/]
                {string.Join("\n", listings
                    .Where(l => l.Delta.HasValue && l.Delta > 0)
                    .OrderByDescending(l => l.Delta)
                    .Take(5)
                    .Select((l, i) => $"{i + 1,2}. {l.Artist.EscapeMarkup()} - {l.Title.EscapeMarkup()} (↑{l.Delta})"))}
                
                [red]📉 Biggest Drops:[/]
                {string.Join("\n", listings
                    .Where(l => l.Delta.HasValue && l.Delta < 0)
                    .OrderBy(l => l.Delta)
                    .Take(5)
                    .Select((l, i) => $"{i + 1,2}. {l.Artist.EscapeMarkup()} - {l.Title.EscapeMarkup()} (↓{Math.Abs(l.Delta!.Value)})"))}
                """))
            .Header("Statistics")
            .Border(BoxBorder.Rounded)
            .BorderColor(Color.Blue);

        AnsiConsole.Write(panel);
    }

    private static string GetChangeText(TrackListing listing)
    {
        if (listing.Delta.HasValue)
        {
            return listing.Delta switch
            {
                0 => "[dim]-[/]",
                > 0 => $"[green]↑{listing.Delta}[/]",
                _ => $"[red]↓{Math.Abs(listing.Delta.Value)}[/]"
            };
        }
        else
        {
            return listing.IsRecurring
                ? "[yellow]↻[/]"
                : "[yellow]★[/]";
        }
    }
}
