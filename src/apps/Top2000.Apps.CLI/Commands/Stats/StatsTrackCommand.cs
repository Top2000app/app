using Top2000.Features;
using Top2000.Features.TrackInformation;
using Spectre.Console.Rendering;

namespace Top2000.Apps.CLI.Commands.Stats;

public class StatsTrackCommand(Top2000Services top2000Services) : CommandBase("track", "Show statistics for a track")
{
    protected override List<Symbol> Symbols =>
    [
        new Option<int>("--edition")
        {
            Description = "Specify the editions year",
        },
        new Option<int>("--position")
        {
            Description = "Specify the position in the edition",
        },
        new Option<int>("--track-id", "--id")
        {
            Description = "Specify the track ID",
        },
        new Option<bool>("--force-all-listings")
        {
            Description = "Force showing all listings even when the console width is small",
        }
    ];


    protected override async Task ExecuteAsync(ParseResult result, CancellationToken token)
    {
        var trackId = await FindTrackIdAsync(result);
        
        if (trackId is not null)
        {
            var forceAllListings = result.GetValue<bool>("--force-all-listings");
            await DisplayTrackStatisticsAsync(forceAllListings, trackId.Value);
        }
    }

    private async Task DisplayTrackStatisticsAsync(bool forceAllListings, int trackId)
    {
        var trackDetails = await top2000Services.TrackDetailsAsync(trackId);

        if (trackDetails.Listings.Count == 0)
        {
            AnsiConsole.MarkupLine($"[red]Unable to find any statistics for track[/]");
            return;
        }

        AnsiConsole.Clear();
        ShowTrackInformation(trackDetails);
        ShowListings(forceAllListings, trackDetails);
        ShowListingChart(trackDetails);
        ShowListingStatistics(trackDetails);
    }

    private static void ShowTrackInformation(TrackDetails trackDetails)
    {
        AnsiConsole.MarkupLine(trackDetails.Title);
        AnsiConsole.MarkupLine($"[red]{trackDetails.Artist} ({trackDetails.RecordedYear})[/]");
        AnsiConsole.WriteLine();
        AnsiConsole.Write(new Rule("[green]Positions[/]").RuleStyle("grey").LeftJustified());
    }

    private static void ShowListingStatistics(TrackDetails trackDetails)
    {
        var statsGrid = new Table { Border = TableBorder.None }
            .HideHeaders()
            .AddColumn("")
            .AddColumn("")
            .AddColumn("");

        statsGrid.AddRow(
            "Highest Listing",
            $"[red]{trackDetails.Highest.Position}[/]",
            $"[grey]({trackDetails.Highest.Edition})[/]"
        );

        statsGrid.AddRow(
            "Lowest Listing",
            $"[red]{trackDetails.Lowest.Position}[/]",
            $"[grey]({trackDetails.Lowest.Edition})[/]"
        );

        statsGrid.AddRow(
            "First Listing",
            $"[red]{trackDetails.First.Position?.ToString() ?? "-"}[/]",
            $"[grey]({trackDetails.First.Edition})[/]"
        );

        statsGrid.AddRow(
            "Last Listing",
            $"[red]{trackDetails.Latest.Position}[/]",
            $"[grey]({trackDetails.Latest.Edition})[/]"
        );

        AnsiConsole.Write(statsGrid);

        var playTimeText = "-";
        var playTime = trackDetails.Latest.LocalUtcDateAndTime;
        if (playTime is not null)
        {
            var playTimeValue = playTime.Value;
            var plusOne = playTimeValue.AddHours(1);
            playTimeText = $"{playTimeValue:dddd d MMMM yyyy HH:mm} - {plusOne:HH:mm}";
        }

        AnsiConsole.MarkupLine($"[grey]                {playTimeText}[/]");
        AnsiConsole.WriteLine();
    }

    private static void ShowListingChart(TrackDetails trackDetails)
    {
        AnsiConsole.Write(new BarChart()
            .Width(100)
            .WithMaxValue(trackDetails.AppearancesPossible)
            .UseValueFormatter(_ => $"{trackDetails.Appearances}/{trackDetails.AppearancesPossible}")
            .AddItem("   In Top 2000", trackDetails.Appearances, Color.Red));

        AnsiConsole.Write(new BarChart()
            .Width(100)
            .WithMaxValue(trackDetails.Listings.Count)
            .UseValueFormatter(_ => $"{trackDetails.Appearances}/{trackDetails.Listings.Count}")
            .AddItem("Since Top 2000", trackDetails.Appearances, Color.Red));
        
        AnsiConsole.WriteLine();
    }

    private static void ShowListings(bool forceAllListings, TrackDetails trackDetails)
    {
        var listingsToShow = trackDetails.Listings.ToList();
        if (!forceAllListings)
        {
            const int minCharsPerColumn = 6;
            var maxColumns =  Math.Max(1, AnsiConsole.Console.Profile.Width / minCharsPerColumn);
            var itemsTillFirst = 1 + trackDetails.Listings
                .TakeWhile(listing => listing.Status != ListingStatus.New)
                .Count();

            var expectedRows = (int)Math.Ceiling((double)itemsTillFirst / maxColumns);
            var maxShownListings = expectedRows * maxColumns;

            listingsToShow = listingsToShow.Take(maxShownListings).ToList();
        }
     
        var columnPanels = new List<Panel>();
        foreach (var listing in listingsToShow)
        {
            var statusText = listing.Status switch
            {
                ListingStatus.New => $"[yellow]{UnicodeSymbols.New}[/]",
                ListingStatus.Decreased => $"[red]{UnicodeSymbols.Down}{Math.Abs(listing.Offset!.Value)}[/]",
                ListingStatus.Increased => $"[green]{UnicodeSymbols.Up}{Math.Abs(listing.Offset!.Value)}[/]",
                ListingStatus.Back => $"[yellow]{UnicodeSymbols.Recurring}[/]",
                ListingStatus.Unchanged => $"[grey]{UnicodeSymbols.Equal}[/]",
                _ => $"[dim]-[/]",
            };

            var positionText = listing.Position.HasValue ? listing.Position.Value.ToString() : "-";
            
            var content = $"[bold]{listing.Edition}[/]\n{statusText}\n{positionText}";
            columnPanels.Add(new Panel(new Markup(content))
            {
                Border = BoxBorder.None,
                Padding = new Padding(0, 0, 1, 1)
            });
        }

        AnsiConsole.Write(new Columns(columnPanels)
        {
            Expand = false
        });
    }

    private async Task<int?> FindTrackIdAsync(ParseResult result)
    {
        var trackId = result.GetValue<int>("--track-id");

        if (trackId != 0)
        {
            return trackId;
        }
        
        var edition = result.GetValue<int>("--edition");
        var position = result.GetValue<int>("--position");
        
        if (position == 0 || edition == 0)
        {
            AnsiConsole.MarkupLine("[red]Error:[/] You must specify either --track-id or both --edition and --position.");
            return null;
        }

        var listings = await top2000Services.AllListingsOfEditionAsync(edition);
        
        if (listings.Count == 0)
        {
            AnsiConsole.MarkupLine($"[red]Error:[/] No listings found for edition {edition}.");
            return null;
        }
        
        var listing = listings.FirstOrDefault(l => l.Position == position);
        if (listing is null)
        {
            AnsiConsole.Markup($"[red]Error:[/] No listing found for position {position}.");
            return null;
        }
        
        return listing.TrackId;
    }
}