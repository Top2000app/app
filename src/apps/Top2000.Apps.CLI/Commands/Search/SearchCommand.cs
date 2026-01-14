using Microsoft.EntityFrameworkCore;
using Top2000.Apps.CLI.Commands.Show;
using Top2000.Apps.CLI.Database;
using Top2000.Features;
using Top2000.Features.Searching;

namespace Top2000.Apps.CLI.Commands.Search;

public class SearchCommand(Top2000Services top2000Services) : CommandBase("search", "Search for tracks in the database")
{
    protected override async Task ExecuteAsync(ParseResult result, CancellationToken token)
    {
        var query = result.GetRequiredValue<string>("query");
        var order = result.GetRequiredValue<OrderBy>("--order");
        var showIds = result.GetValue<bool>("--showIds");

        var latest = (await top2000Services.AllEditionsAsync(token)).First();
        
        var sort = new CustomSort(order);
        var group = new GroupByNothing();
        
        var searchResultsGrouped = await top2000Services.SearchAsync(query, latest.Year, sort, group, token);
        var searchResults = searchResultsGrouped.First().ToList();
        
        if (!searchResults.Any())
        {
            AnsiConsole.MarkupLine($"[yellow]No results found for query: '{query.EscapeMarkup()}'[/]");
        }

        // Display results in a table
        var table = new Table()
            .Title($"Search Results for: '{query.EscapeMarkup()}'")
            .Border(TableBorder.Rounded)
            .BorderColor(Color.Grey);

        if (showIds)
        {
            table.AddColumn("[bold]ID[/]");
        }
        table.AddColumn("[bold]Year[/]");
        table.AddColumn("[bold]Title[/]");
        table.AddColumn("[bold]Artist[/]");
        table.AddColumn($"[bold]Position in {latest.Year}[/]");

        foreach (var searchResult in searchResults)
        {
            if (showIds)
            {
                table.AddRow(
                    searchResult.Id.ToString(),
                    searchResult.RecordedYear.ToString(),
                    searchResult.Title.EscapeMarkup(),
                    searchResult.Artist.EscapeMarkup(),
                    searchResult.Position?.ToString() ?? "-"
                );
            }
            else
            {
                table.AddRow(
                    searchResult.RecordedYear.ToString(),
                    searchResult.Title.EscapeMarkup(),
                    searchResult.Artist.EscapeMarkup(),
                    searchResult.Position?.ToString() ?? "-"
                );
            }
        }

        AnsiConsole.Write(table);
        AnsiConsole.MarkupLine($"[dim]Found {searchResults.Count} result(s)[/]");
    }
    
    private class CustomSort(OrderBy orderBy): ISort
    {
        public IOrderedEnumerable<SearchedTrack> Sort(IEnumerable<SearchedTrack> tracks)
        {
            return orderBy switch
            {
                OrderBy.Artist => tracks.OrderBy(track => track.Artist),
                OrderBy.ArtistDescending => tracks.OrderByDescending(track => track.Artist),
                OrderBy.Title => tracks.OrderBy(track => track.Title),
                OrderBy.TitleDescending => tracks.OrderByDescending(track => track.Title),
                OrderBy.Year => tracks.OrderBy(track => track.RecordedYear),
                OrderBy.YearDescending => tracks.OrderByDescending(track => track.RecordedYear),
                OrderBy.IdDescending => tracks.OrderByDescending(track => track.Id),
                OrderBy.LatestPosition => tracks.OrderBy(track => track.Position),
                OrderBy.LatestPositionDescending => tracks.OrderByDescending(track => track.Position),
                _ => tracks.OrderBy(track => track.Id)
            };
        }
    }


    protected override List<Symbol> Symbols =>
    [
        new Argument<string>("query")
        {
            Description = "Search query string"
        },
        new Option<bool>("--showIds")
        {
            Description = "Whether to show IDs in the results",
            DefaultValueFactory = (_) => false
        },
        new Option<OrderBy>("--order")
        {
            Description = "Order the results by the specified field",
            DefaultValueFactory = (_) => OrderBy.Title
        }
    ];

    private enum OrderBy
    {
        Year,
        Title,
        Artist,
        Id,
        LatestPosition,
        YearDescending,
        TitleDescending,
        ArtistDescending,
        IdDescending,
        LatestPositionDescending
    }
}