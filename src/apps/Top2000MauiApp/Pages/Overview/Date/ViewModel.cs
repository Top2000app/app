using Top2000.Features.AllEditions;
using Top2000.Features.AllListingsOfEdition;
using Top2000MauiApp.Common;

namespace Top2000MauiApp.Pages.Overview.Date;

public partial class ViewModel : ObservableObject
{
    private readonly IMediator _mediator;

    public ViewModel(IMediator mediator)
    {
        _mediator = mediator;
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
        var editions = await _mediator.Send(new AllEditionsRequest());
        this.SelectedEditionYear = editions.First().Year;

        await this.LoadAllListingsAsync();
    }

    public async Task LoadAllListingsAsync()
    {
        var tracks = await _mediator.Send(new AllListingsOfEditionRequest { Year = this.SelectedEditionYear });

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
