using Top2000.Features;
using Top2000.Features.Listings;

namespace Top2000.Apps.CLI.Commands.Stats;

public class StatsListingCommand(Top2000Services top2000Services) : CommandBase("edition", "Show a specific Top 2000 edition")
{
    protected override List<Symbol> Symbols =>
    [
        new Argument<int>("year")
        {
            Description = "Year of the edition to show",
            Arity =  ArgumentArity.ExactlyOne
        }
    ];

    protected override async Task ExecuteAsync(ParseResult result, CancellationToken token)
    {
        var edition = result.GetRequiredValue<int>("year");

        var listings = await top2000Services.AllListingsOfEditionAsync(edition, token);
        
        AnsiConsole.Clear();
        if (listings.Count > 0)
        {
            var theListing = (await top2000Services.AllEditionsAsync(token))
                .First(e => e.Year == edition);
            var count = listings.Count;
            
            var countOfIncreased = listings.Count(x => x.DeltaType == TrackListingDeltaType.Increased);
            var highestIncrease = listings
                .Where(x => x.DeltaType == TrackListingDeltaType.Increased)
                .OrderByDescending(x => x.Delta)
                .FirstOrDefault();
            
            var countOfDecreased = listings.Count(x => x.DeltaType == TrackListingDeltaType.Decreased);
            var highestDecrease = listings
                .Where(x => x.DeltaType == TrackListingDeltaType.Decreased)
                .OrderBy(x => x.Delta)
                .FirstOrDefault();
            
            var countOfNew = listings.Count(x => x.DeltaType == TrackListingDeltaType.New);
            var highestNew = listings
                .Where(x => x.DeltaType == TrackListingDeltaType.New)
                .OrderBy(x => x.Position)
                .FirstOrDefault();
            
            var countOfReturned = listings.Count(x => x.DeltaType == TrackListingDeltaType.Recurring);
            var highestReturned = listings
                .Where(x => x.DeltaType == TrackListingDeltaType.Recurring)
                .OrderBy(x => x.Position)
                .FirstOrDefault();

            var countOfUnchanged = listings.Count(x => x.DeltaType == TrackListingDeltaType.NoChange);

            // Display dashboard
            var rule = new Rule($"[bold yellow]Top2000 - Statistics Dashboard[/]")
            {
                Justification = Justify.Center
            };
            AnsiConsole.Write(rule);
            AnsiConsole.WriteLine();

            var localTimeZone = TimeZoneInfo.Local;
            var timeZoneName = localTimeZone.DisplayName;
            
            // Total count panel
            var totalContent = new Markup(
                    $"[bold white]{count}[/] tracks\n" +
                    $"Start: {theListing.LocalStartDateAndTime:f} [dim]{timeZoneName}[/]\n" +
                    $"End: {theListing.LocalEndDateAndTime:f} [dim]{timeZoneName}[/]"
            );
            var totalPanel = new Panel(totalContent)
            {
                Header = new PanelHeader($"[bold cyan]Edition {edition}[/]", Justify.Center),
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Cyan1)
            };
            AnsiConsole.Write(totalPanel);
            AnsiConsole.WriteLine();

            // Increased tracks
            var increasedContent = new Markup($"[bold green]{UnicodeSymbols.Up} {countOfIncreased}[/] tracks increased\n");
            if (highestIncrease != null)
            {
                increasedContent = new Markup(
                    $"[bold green]{UnicodeSymbols.Up} {countOfIncreased}[/] tracks increased\n\n" +
                    $"[dim]Highest increase:[/]\n" +
                    $"[yellow]#{highestIncrease.Position}[/] " +
                    $"[green]+{highestIncrease.Delta}[/]\n" +
                    $"{highestIncrease.Title}\n" +
                    $"{highestIncrease.Artist}");
            }
            var increasedPanel = new Panel(increasedContent)
            {
                Header = new PanelHeader(" [bold green]Increased[/]"),
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Green)
            };
            
            // Decreased tracks
            var decreasedContent = new Markup($"[bold red]{UnicodeSymbols.Down} {countOfDecreased}[/] tracks decreased\n");
            if (highestDecrease != null)
            {
                decreasedContent = new Markup(
                    $"[bold red]{UnicodeSymbols.Down} {countOfDecreased}[/] tracks decreased\n\n" +
                    $"[dim]Highest decrease:[/]\n" +
                    $"[yellow]#{highestDecrease.Position}[/] " +
                    $"[red]{highestDecrease.Delta}[/]\n" +
                    $"{highestDecrease.Title}\n" +
                    $"{highestDecrease.Artist}");
            }
            var decreasedPanel = new Panel(decreasedContent)
            {
                Header = new PanelHeader(" [bold red]Decreased[/]"),
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Red)
            };
            
            // New tracks
            var newContent = new Markup($"[bold yellow] {UnicodeSymbols.New} {countOfNew}[/] new tracks\n");
            if (highestNew != null)
            {
                newContent = new Markup(
                    $"[bold yellow]{UnicodeSymbols.New} {countOfNew}[/] new tracks\n\n" +
                    $"[dim]Highest new entry:[/]\n" +
                    $"[yellow]#{highestNew.Position}[/]\n" +
                    $"{highestNew.Title}\n" +
                    $"{highestNew.Artist}");
            }
            var newPanel = new Panel(newContent)
            {
                Header = new PanelHeader($"{UnicodeSymbols.New} [bold yellow]New Entries[/]"),
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Yellow)
            };

            // Recurring tracks
            var recurringContent = new Markup($"[bold magenta]{UnicodeSymbols.Recurring} {countOfReturned}[/] tracks returned\n");
            if (highestReturned != null)
            {
                recurringContent = new Markup(
                    $"[bold magenta]{UnicodeSymbols.Recurring} {countOfReturned}[/] tracks returned\n\n" +
                    $"[dim]Highest return:[/]\n" +
                    $"[yellow]#{highestReturned.Position}[/]\n" +
                    $"{highestReturned.Title}\n" +
                    $"{highestReturned.Artist}");
            }
            var recurringPanel = new Panel(recurringContent)
            {
                Header = new PanelHeader("[bold magenta]Recurring[/]"),
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Magenta)
            };

            // Unchanged tracks
            var unchangedPanel = new Panel(new Markup($"[bold white] {countOfUnchanged}[/] tracks unchanged"))
            {
                Header = new PanelHeader(" [bold white]No Change[/]", Justify.Center),
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Grey)
            };
            
            var columns = new Columns([increasedPanel, decreasedPanel,newPanel, recurringPanel, unchangedPanel])
            {
                Padding = new Padding(2,0),
                Expand = false
            };
            
            AnsiConsole.Write(columns);
            AnsiConsole.WriteLine();
        }
    }
}