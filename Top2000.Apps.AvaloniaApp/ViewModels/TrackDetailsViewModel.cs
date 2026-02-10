using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using Top2000.Apps.AvaloniaApp.Assets;
using Top2000.Features.TrackInformation;

namespace Top2000.Apps.AvaloniaApp.ViewModels;

public partial class DesignTimeTrackDetailsViewModel : TrackDetailsViewModel
{
    public DesignTimeTrackDetailsViewModel()
    {
        var listings = new List<ListingInformation>
        {
            new()
            {
                Edition = 2023,
                Position = null,
                Delta = null,
                Status = ListingStatus.Unknown
            },
            new()
            {
                Edition = 2022,
                Position = 40,
                Delta = -1,
                Status = ListingStatus.Decreased
            },
            new()
            {
                Edition = 2021,
                Position = 39,
                Delta = 2091,
                Status = ListingStatus.Increased
            },
            new()
            {
                Edition = 2020,
                Position = 2130,
                Delta = null,
                Status = ListingStatus.Unchanged
            },
            new()
            {
                Edition = 2019,
                Position = 2130,
                Delta = null,
                Status = ListingStatus.Back
            },
            new()
            {
                Edition = 2018,
                Position = null,
                Delta = null,
                Status = ListingStatus.NotListed
            },
            new()
            {
                Edition = 2017,
                Position = 500,
                Delta = null,
                Status = ListingStatus.New
            },
            new()
            {
                Edition = 2017,
                Position = null,
                Delta = null,
                Status = ListingStatus.NotAvailable
            }
        };
        
        Title = "This is a design time track title";
        Artist = "This is a deign time artist";
        RecordedYear = 1971;
        Listings = listings
            .Select(x => new TrackDetailsListingViewModel
            {
                Edition = x.Edition,
                Position = x.Position,
                Delta = TrackDetailsListingViewModel.ConvertDeltaToString(x),
                DeltaSymbol = TrackDetailsListingViewModel.ConvertDeltaToSymbol(x),
                DeltaSymbolColour = new SolidColorBrush(TrackDetailsListingViewModel.ConvertDeltaSymbolColour(x)),
                DeltaFontSize = TrackDetailsListingViewModel.ConvertDeltaFontSize(x),
                Status = x.Status
            })
            .ToList();
    }
}

public partial class TrackDetailsViewModel : ViewModelBase
{
    public required string Title { get; init; }
    public required string Artist { get; init; }
    public required int RecordedYear { get; init; }
    public required List<TrackDetailsListingViewModel> Listings { get; init; }

    [ObservableProperty] private TrackDetailsListingViewModel _selectedListing;
    
    public required MainWindowViewModel ParentMainWindowViewModel { get; init; }
    
    public TrackDetailsListingViewModel Highest => Listings
        .Where(x => x.Position.HasValue)
        .OrderBy(x => x.Position)
        .ThenBy(x => x.Edition)
        .First();

    public TrackDetailsListingViewModel Lowest => Listings
        .Where(x => x.Position.HasValue)
        .OrderBy(x => x.Position)
        .ThenBy(x => x.Edition)
        .Last();

    public TrackDetailsListingViewModel First => Listings.Single(x => x.Status == ListingStatus.New);

    public TrackDetailsListingViewModel Latest => Listings.First(x => x.Position.HasValue);

    public int Appearances => Listings.Count(x => x.Position.HasValue);

    public int AppearancesPossible => Listings.Count(x => x.Status != ListingStatus.NotAvailable);

    public double SinceRelease => ((double)Appearances / AppearancesPossible) * 360;
    
    public double InTop2000 => ((double)Appearances / Listings.Count) * 360d;

    async partial void OnSelectedListingChanged(TrackDetailsListingViewModel? value)
    {
        if (value is not null)
        {
            ParentMainWindowViewModel?.ChangeSelectedEditionAsync(value.Edition, value.Position);
        }
    }
}

public class TrackDetailsListingViewModel
{
    public required int Edition { get; init; }
    public string PositionString => Position?.ToString() ?? "-";
    public required int? Position { get; init; }
    public required string Delta { get; init; }
    public required string DeltaSymbol { get; init; }
    public required Brush DeltaSymbolColour { get; init; }
    public required double DeltaFontSize { get; init; }
    public required ListingStatus Status { get; init; }

    public static double ConvertDeltaFontSize(ListingInformation listing)
    {
        return listing.Status switch
        {
            ListingStatus.Back => 20,
            ListingStatus.Decreased => 11,
            ListingStatus.Increased => 11,
            ListingStatus.New => 20,
            _ => 11
        };
    }

    public static string ConvertDeltaToSymbol(ListingInformation listing)
    {
        return listing.Status switch
        {
            ListingStatus.Back => Symbols.BackInList,
            ListingStatus.Increased => Symbols.Up,
            ListingStatus.Decreased => Symbols.Down,
            ListingStatus.New => Symbols.Flag,
            ListingStatus.Unchanged => Symbols.Same,
            _ => Symbols.Minus
        };
    }

    public static string ConvertDeltaToString(ListingInformation listing)
    {
        return listing.Delta.HasValue
            ? Math.Abs(listing.Delta.Value).ToString()
            : string.Empty;
    }

    public static Color ConvertDeltaSymbolColour(ListingInformation listing)
    {
        return listing.Status switch
        {
            ListingStatus.Increased => Colours.GreenColour,
            ListingStatus.Decreased => Colours.RedColour,
            ListingStatus.New or ListingStatus.Back => Colours.YellowColour,
            _ => Colours.GreyColour
        };
    }
}