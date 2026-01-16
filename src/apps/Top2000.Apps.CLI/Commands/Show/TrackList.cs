using Top2000.Features.Listings;

namespace Top2000.Apps.CLI.Commands.Show;

public static class TrackListView
{
    public static void DisplayTable(List<TrackListing> listings, string title)
    {
        AnsiConsole.Clear();
        var table = new Table()
            .Title(title)
            .Border(TableBorder.Rounded)
            .BorderColor(Color.Grey);

        table.AddColumn(new TableColumn("[bold]#[/]"));
        table.AddColumn(new TableColumn($"[bold]{UnicodeSymbols.Delta}[/]"));
        table.AddColumn("[bold]Title[/]");
        table.AddColumn("[bold]Artist[/]");

        foreach (var listing in listings)
        {
            var changeText = GetChangeText(listing);
            
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
    
    private static string GetChangeText(TrackListing listing)
    {
        return listing.DeltaType switch
        {
            TrackListingDeltaType.NoChange => $"[dim]{UnicodeSymbols.Equal}[/]",
            TrackListingDeltaType.Increased => $"[green]{UnicodeSymbols.Up}{listing.Delta}[/]",
            TrackListingDeltaType.Decreased => $"[red]{UnicodeSymbols.Down}{Math.Abs(listing.Delta)}[/]",
            TrackListingDeltaType.New => $"[yellow]{UnicodeSymbols.New}[/]",
            TrackListingDeltaType.Recurring => $"[yellow]{UnicodeSymbols.Recurring}[/]",
            _ => $"[dim]{UnicodeSymbols.Dash}[/]"
        };
    }
}