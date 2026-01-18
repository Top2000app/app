using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using Top2000.Features;
using Top2000.Features.Editions;
using Top2000.Features.Listings;
using Top2000.Features.TrackInformation;

namespace Top2000.Apps.AvaloniaApp.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly ITop2000Services _top2000Services;

    public MainWindowViewModel()
    {
        _top2000Services = new MockupTop2000Services();
    }
    
    public MainWindowViewModel(ITop2000Services services)
    {
        _top2000Services = services;
    }

    [ObservableProperty] 
    public List<ITrackListingViewModel> listings = [];
    
    [ObservableProperty]
    public Edition? selectedEdition;
  
    [ObservableProperty]
    public string title = "Top2000";

    [ObservableProperty]
    public bool isLoaded;

    [ObservableProperty] public TrackDetails selectedListing = new TrackDetails
    {
        Title = "",
        Artist = "",
        RecordedYear = 0,
        Listings = []
    };
    
    [ObservableProperty]
    public ITrackListingViewModel? selectedItem;
    
    async partial void OnSelectedItemChanged(ITrackListingViewModel? value)
    {
        if (value is not null)
        {
            if (value is TrackListingViewModel trackListing)
            {
                var details = await _top2000Services.TrackDetailsAsync(trackListing.TrackId);
                SelectedListing = details;
                // Title = $"{trackListing.Title} - {trackListing.Artist}";
            }

            if (value is TrackListingViewModelGroup group)
            {
                Title = group.GroupName;
            }
        }
    }
    
    public async Task InitialiseViewModelAsync()
    {
        var editions = await _top2000Services.AllEditionsAsync();
        SelectedEdition = editions.First();
        Title = "TOP2000 - " + this.SelectedEdition.Year;

        await this.LoadAllListingsAsync();
        
        IsLoaded = true;
    }
   

    private async Task LoadAllListingsAsync()
    {
        if (this.SelectedEdition is null)
        {
            return;
        }

        var items = new List<ITrackListingViewModel>();
        
        var result = (await _top2000Services.AllListingsOfEditionAsync(this.SelectedEdition.Year))
            .GroupByPosition();

        foreach (var group in result)
        {
            items.Add(new TrackListingViewModelGroup
            {
                GroupName =  group.Key,
            });

            var grouped = group
                .Select(TransformTrackListingViewModel)
                .ToList();
            
            items.AddRange(grouped);
        }

        Listings = items;
    }

    private ITrackListingViewModel TransformTrackListingViewModel(TrackListing x)
    {
        return new TrackListingViewModel
        {
            TrackId = x.TrackId,
            Artist = x.Artist,
            Title = x.Title,
            Delta = TrackListingViewModel.ConvertDeltaToString(x),
            DeltaFontSize = TrackListingViewModel.ConvertDeltaFontSize(x),
            PositionString = x.Position.ToString(),
            Position = x.Position,
            DeltaSymbol = TrackListingViewModel.ConvertDeltaToSymbol(x),
            DeltaSymbolColour = new SolidColorBrush(TrackListingViewModel.ConvertDeltaSymbolColour(x)),
            LocalPlayDateTime = x.PlayUtcDateAndTime.ToLocalTime()
        };
    }
}