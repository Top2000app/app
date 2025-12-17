using Top2000.Features;
using Top2000MauiApp.Common;

namespace Top2000MauiApp.Pages.Overview.Date;

public partial class ViewModel : ObservableObject
{
    private readonly Top2000Services _top2000Services;

    public ViewModel(Top2000Services top2000Services)
    {
        _top2000Services = top2000Services;
        Listings = [];
        Dates = [];
    }

    public ObservableGroupedList<DateTime, TrackListingViewModel> Listings { get; }

    public ObservableGroupedList<DateTime, DateTime> Dates { get; }

    [ObservableProperty]
    public int selectedEditionYear;

    [ObservableProperty]
    public TrackListingViewModel? selectedListing;


    public async Task InitialiseViewModelAsync()
    {
        var editions = await _top2000Services.AllEditionsAsync();
        this.SelectedEditionYear = editions.First().Year;

        await this.LoadAllListingsAsync();
    }

    public async Task LoadAllListingsAsync()
    {
        var tracks = await _top2000Services.AllListingsOfEditionAsync(this.SelectedEditionYear);

        var listings = tracks
             .Select(x => new TrackListingViewModel
             {
                 TrackId = x.TrackId,
                 Artist = x.Artist,
                 Title = x.Title,
                 Delta = TrackListingViewModel.ConvertDeltaToString(x),
                 DeltaFontSize = TrackListingViewModel.ConvertDeltaFontSize(x),
                 PositionString = x.Position.ToString(),
                 Position = x.Position,
                 DeltaSymbol = TrackListingViewModel.ConvertDeltaToSymbol(x),
                 DeltaSymbolColour = TrackListingViewModel.ConvertDeltaSymbolColour(x),
                 LocalPlayDateTime = x.PlayUtcDateAndTime.ToLocalTime()
             })
            .OrderByDescending(x => x.Position)
            .GroupBy(x => x.LocalPlayDateTime);

        var dates = listings
            .Select(x => x.Key)
            .GroupBy(this.LocalPlayDate);

        this.Listings.ClearAddRange(listings);
        this.Dates.ClearAddRange(dates);
    }

    private DateTime LocalPlayDate(DateTime arg) => arg.Date;
}
