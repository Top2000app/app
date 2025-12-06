using Top2000.Features.TrackInformation;
using Top2000MauiApp.Common;
using Top2000MauiApp.Themes;

namespace Top2000MauiApp.Pages.TrackInformation;

public partial class ViewModel : ObservableObject
{
    private readonly IMediator mediator;

    public ViewModel(IMediator mediator)
    {
        this.mediator = mediator;
        this.Listings = [];
    }

    [ObservableProperty]
    public string? title;

    [ObservableProperty]
    public string? artist;

    [ObservableProperty]
    public string? artistWithYear;

    public ObservableList<ListingInformationViewModel> Listings { get; }

    [ObservableProperty]
    public int? totalListings;

    [ObservableProperty]
    public ListingInformation? highest;

    [ObservableProperty]
    public ListingInformation? lowest;


    [ObservableProperty]
    public ListingInformation? latest;


    [ObservableProperty]
    public ListingInformation? first;


    [ObservableProperty]
    public int? appearances;


    [ObservableProperty]
    public bool? isLatestListed;


    [ObservableProperty]
    public int? appearancesPossible;


    [ObservableProperty]
    public int? appearancesPossiblePercentage;


    [ObservableProperty]
    public int? totalTop2000Percentage;

    [ObservableProperty]
    public string? localUtcDateAndTime;

    public async Task LoadTrackDetailsAsync(int trackId)
    {
        var track = await mediator.Send(new TrackInformationRequest { TrackId = trackId });

        this.Title = track.Title;
        this.ArtistWithYear = $"{track.Artist} ({track.RecordedYear})";
        this.Artist = track.Artist;
        this.Highest = track.Highest;
        this.Lowest = track.Lowest;
        this.Latest = track.Latest;
        this.First = track.First;
        this.Appearances = track.Appearances;
        this.AppearancesPossible = track.AppearancesPossible;
        this.IsLatestListed = track.Listings.First().Status != ListingStatus.NotListed;
        this.Listings.Clear();


        var listings = track.Listings
            .Select(x => new ListingInformationViewModel()
            {
                Edition = x.Edition,
                StatusSymbol = ToStatusSymbol(x.Status),
                TextColour = Convert(x.Status),
                Offset = ConvertToOffset(x.Offset),
                Position = ConvertPosition(x.Position),
            })
            .ToList();

        this.Listings.ClearAddRange(listings);

        this.AppearancesPossiblePercentage = 100 * this.Appearances / this.AppearancesPossible;
        this.TotalTop2000Percentage = 100 * this.Appearances / this.Listings.Count;

        this.TotalListings = this.Listings.Count;
        this.LocalUtcDateAndTime = ConvertToLocalTime(track.Latest.PlayUtcDateAndTime?.ToLocalTime());
    }

    public static string ConvertPosition(int? value)
    {
        return value.HasValue
           ? value.Value.ToString(App.NumberFormatProvider)
           : "-";
    }

    public static Color Convert(ListingStatus value) => value switch
    {
        ListingStatus.New => Colours.YellowColour,
        ListingStatus.Back => Colours.YellowColour,
        ListingStatus.Decreased => Colours.RedColour,
        ListingStatus.Increased => Colours.GreenColour,
        _ => Colours.GreyColour,
    };

    public static string ToStatusSymbol(ListingStatus value) => value switch
    {
        ListingStatus.New => Symbols.Flag,
        ListingStatus.Decreased => Symbols.Down,
        ListingStatus.Increased => Symbols.Up,
        ListingStatus.Unchanged => Symbols.Same,
        ListingStatus.Back => Symbols.BackInList,
        _ => Symbols.Minus
    };

    public static string ConvertToOffset(int? offset)
    {
        return offset.HasValue && offset.Value != 0
           ? Math.Abs(offset.Value).ToString(App.NumberFormatProvider)
           : string.Empty;
    }

    public static string ConvertToLocalTime(DateTime? value)
    {
        if (value is null)
        {
            return string.Empty;
        }
        
        var hour = value.Value.TimeOfDay.Hours;
        var untilHour = hour + 1;

        if (untilHour > 24)
        {
            untilHour = 0;
        }

        var date = value.Value.ToString("dddd d MMMM yyyy", App.DateTimeFormatProvider);

        return $"{date} {hour}:00 - {untilHour}:00";
    }
}
