using Top2000.Features.AllEditions;
using Top2000.Features.AllListingsOfEdition;
using Top2000MauiApp.Common;

namespace Top2000MauiApp.Pages.Overview.Position;

public partial class ViewModel : ObservableObject
{
    private readonly IMediator mediator;

    public ViewModel(IMediator mediator)
    {
        this.mediator = mediator;
        this.Listings = [];
        this.Editions = [];
    }

    public ObservableGroupedList<string, TrackListingViewModel> Listings { get; }

    [ObservableProperty]
    public TrackListingViewModel? selectedListing;

    public ObservableList<Edition> Editions { get; }

    [ObservableProperty]
    public Edition? selectedEdition;

    [ObservableProperty]
    public int selectedEditionYear;

    [ObservableProperty]
    public int countOfItems;

    public async Task InitialiseViewModelAsync()
    {
        var editions = await mediator.Send(new AllEditionsRequest());
        this.SelectedEdition = editions.First();
        this.SelectedEditionYear = this.SelectedEdition.Year;
        this.Editions.ClearAddRange(editions);

        await this.LoadAllListingsAsync();
    }

    public async Task InitialiseViewModelAsync(Edition newEdition)
    {
        this.SelectedEdition = newEdition;
        this.SelectedEditionYear = this.SelectedEdition.Year;

        await this.LoadAllListingsAsync();
    }

    public async Task LoadAllListingsAsync()
    {
        if (this.SelectedEdition is null)
        {
            return;
        }

        var result = await mediator.Send(new AllListingsOfEditionRequest { Year = this.SelectedEdition.Year });

        var listings = result
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
            .GroupBy(x => Position(x, result.Count))
            .ToList();

        this.CountOfItems = result.Count;
        this.Listings.ClearAddRange(listings);

        this.SelectedListing = null;
    }

    private static string Position(TrackListingViewModel listing, int countOfItems)
    {
        if (listing.Position < 100)
        {
            return "1 - 100";
        }

        if (countOfItems > 2000)
        {
            if (listing.Position >= 2400)
            {
                return "2400 - 2500";
            }
        }
        else if (listing.Position >= 1900)
        {
            return "1900 - 2000";
        }

        int num = listing.Position / 100 * 100;
        int value = num + 100;
        return $"{num} - {value}";
    }
}
