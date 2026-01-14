using System.CommandLine.Parsing;
using Top2000.Data.JsonClientDatabase.Models;
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
        
        editionCommand.Add(new Argument<string>("year")
        {
            Description = "Year of the edition to show",
            Arity = ArgumentArity.ExactlyOne,
        });
        
        editionCommand.Add(new Option<int>("--top")
        {
            Description = "Number of top tracks to show",
        });
        editionCommand.Add(new Option<int>("--skip")
        {
            Description = "Number of tracks to skip from the listing. If --top is specified, skip is ignored.",
        });
        editionCommand.Add(new Option<int>("--take")
        {
            Description =  "Number of tracks to take from the listing. If --top is specified, skip is ignored.",
        });
        
        editionCommand.Add(new Option<bool>("--new")
        {
            Description = "Show tracks that are new to the Top 2000 this edition",
        });
        editionCommand.Add(new Option<bool>("--recurring")
        {
            Description = "Show tracks that are back in the Top 2000 after being absent"
        });
        editionCommand.Add(new Option<bool>("--risers")
        {
            Description = "Show tracks that have increased in position from the previous edition"
        });
        editionCommand.Add(new Option<bool>("--fallers")
        {
            Description = "Show tracks that have decreased in position from the previous edition"
        });
        editionCommand.Add(new Option<bool>("--held")
        {
            Description = "Show tracks that have maintained the same position from the previous edition"
        });
        
        editionCommand.Add(new Option<Ordering>("--order")
        {
            Description = "Order the listing by the specified field",
            DefaultValueFactory = (_) => Ordering.Rank
        });
        
        return editionCommand;
    }

    public enum Ordering
    {
        Rank,
        Title,
        Artist,
        Delta,
        RankDescending,
        TitleDescending,
        ArtistDescending,
        DeltaDescending
    }

    private async Task<int> HandleShowEditionAsync(ParseResult result, CancellationToken token)
    {
        var year = int.Parse(result.GetRequiredValue<string>("year"));

        var showNew = result.GetValue<bool>("--new");
        var showRecurring = result.GetValue<bool>("--recurring");
        var showRisers = result.GetValue<bool>("--risers");
        var showFallers = result.GetValue<bool>("--fallers");
        var showHeld = result.GetValue<bool>("--held");
        var showAll = !showHeld && !showFallers && !showRisers && !showRecurring && !showNew;
        
        var top = result.GetValue<int>("--top");
        var skip = result.GetValue<int>("--skip");
        var take = result.GetValue<int>("--take");
        
        var order = result.GetValue<Ordering>("--order");
        
        var listingsForYear = await _top2000Services.AllListingsOfEditionAsync(year, token);

        if (listingsForYear.Count == 0)
        {
            AnsiConsole.MarkupLine($"[yellow]No listings found for year {year}.[/]");
            return 0;
        }

        var listings = new List<TrackListing>();

        if (showAll)
        {
            listings.AddRange(listingsForYear);
        }
        
        if (showHeld)
        {
            listings.AddRange(listingsForYear.Where(x => x.DeltaType == TrackListingDeltaType.NoChange));
        }

        if (showNew)
        {
            listings.AddRange(listingsForYear.Where(x => x.DeltaType == TrackListingDeltaType.New));
        }

        if (showFallers)
        {
            listings.AddRange(listingsForYear.Where(x => x.DeltaType == TrackListingDeltaType.Decreased));
        }

        if (showRisers)
        {
            listings.AddRange(listingsForYear.Where(x => x.DeltaType == TrackListingDeltaType.Increased));
        }

        if (showRecurring)
        {
            listings.AddRange(listingsForYear.Where(x => x.DeltaType == TrackListingDeltaType.Recurring));
        }
        
        if (top > 0)
        {
            listings = listings.Take(top).ToList();
        }
        else
        {
            listings = skip switch
            {
                > 0 when take > 0 => listings.Skip(skip).Take(take).ToList(),
                < 1 when take > 0 => listings.Take(take).ToList(),
                > 0 when take < 1 => listings.Skip(skip).ToList(),
                _ => listings
            };
        }
      
        listings = order switch
        {
            Ordering.Rank => listings.OrderBy(x => x.Position).ToList(),
            Ordering.Title => listings.OrderBy(x => x.Title).ToList(),
            Ordering.Artist => listings.OrderBy(x => x.Artist).ToList(),
            Ordering.Delta => listings.OrderBy(x => x.Delta).ToList(),
            Ordering.RankDescending => listings.OrderByDescending(x => x.Position).ToList(),
            Ordering.TitleDescending => listings.OrderByDescending(x => x.Title).ToList(),
            Ordering.ArtistDescending => listings.OrderByDescending(x => x.Artist).ToList(),
            Ordering.DeltaDescending => listings.OrderByDescending(x => x.Delta).ToList(),
            _ => listings
        };
        
        TrackListView.DisplayTable(listings, $"Top 2000 {year}");

        return 1;
    }
}
