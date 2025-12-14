using Microsoft.EntityFrameworkCore;
using Spectre.Console;
using System.CommandLine.Parsing;
using Top2000.Apps.CLI.Database;

namespace Top2000.Apps.CLI.Commands.Search;

public class SearchCommandHandler
{
    private readonly Top2000DbContext _dbContext;

    public SearchCommandHandler(Top2000DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> HandleSearchAsync(ParseResult result, CancellationToken token)
    {
        var query = result.GetValue<string>("--query");
        var showIds = result.GetValue<bool>("--showIds");
        
        if (string.IsNullOrWhiteSpace(query))
        {
            AnsiConsole.MarkupLine("[yellow]Please provide a search query using --query parameter.[/]");
            return 1;
        }

        // Search for tracks that match the query in title or artist
        var searchResults = await _dbContext.Tracks
            .Where(t => t.Title.Contains(query) || t.Artist.Contains(query))
            .Select(t => new
            {
                t.Id,
                t.Title,
                t.Artist,
                Year = t.RecordedYear
            })
            .OrderBy(r => r.Year)
            .ThenBy(r => r.Artist)
            .ThenBy(r => r.Title)
            .ToListAsync(token);

        if (!searchResults.Any())
        {
            AnsiConsole.MarkupLine($"[yellow]No results found for query: '{query.EscapeMarkup()}'[/]");
            return 0;
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

        foreach (var searchResult in searchResults)
        {
            if (showIds)
            {
                table.AddRow(
                    searchResult.Id.ToString(),
                    searchResult.Year.ToString(),
                    searchResult.Title.EscapeMarkup(),
                    searchResult.Artist.EscapeMarkup()
                );
            }
            else
            {
                table.AddRow(
                    searchResult.Year.ToString(),
                    searchResult.Title.EscapeMarkup(),
                    searchResult.Artist.EscapeMarkup()
                );
            }
        }

        AnsiConsole.Write(table);
        AnsiConsole.MarkupLine($"[dim]Found {searchResults.Count} result(s)[/]");

        return 0;
    }
}