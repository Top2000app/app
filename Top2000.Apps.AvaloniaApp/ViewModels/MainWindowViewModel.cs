using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Top2000.Apps.AvaloniaApp.Assets;
using Top2000.Features;
using Top2000.Features.Editions;
using Top2000.Features.Listings;
namespace Top2000.Apps.AvaloniaApp.ViewModels;

public interface ICanFilterListings
{
    void UpdateListings();
}


public enum ListingOrder
{
    Ascending,
    Descending,
}

public enum ListingGroup
{
    Position,
    PlayTime
}

public partial class MainWindowViewModel : ViewModelBase, ICanFilterListings
{
    private SortedSet<Edition> Editions { get; set; } = [];
    private HashSet<TrackListing> _originalListings = [];
    private readonly ITop2000Services _top2000Services;

    public MainWindowViewModel()
    {
        _top2000Services = new MockupTop2000Services();
        Filters = FilterCollection.Initialise(this);
    }
    
    public MainWindowViewModel(ITop2000Services services)
    {
        _top2000Services = services;
        Filters = FilterCollection.Initialise(this);
    }

    public FilterCollection Filters { get; set; }
    
    [ObservableProperty] private List<ITrackListingViewModel> _listings = [];
    [ObservableProperty] private List<ITrackListingViewModel> _listingGroups = [];
    [ObservableProperty] private Edition? _selectedEdition;
    [ObservableProperty] private string _title = "Top2000";
    [ObservableProperty] private bool _isLoaded;
    [ObservableProperty] private TrackDetailsViewModel? _selectedListing;
    [ObservableProperty] private ITrackListingViewModel? _selectedItem;
    [ObservableProperty] private ListingOrder _listingOrder = ListingOrder.Ascending;
    [ObservableProperty] private ListingGroup _listingGroup = ListingGroup.Position;
    [ObservableProperty] private string _orderIcon = Symbols.Up;
    [ObservableProperty] private string _groupIcon = Symbols.ListingMenu;
    [ObservableProperty] private bool _canToggleGrouping = true;
    
    public async Task ChangeSelectedEditionAsync(int edition, int? position)
    {
        if (SelectedEdition.Year != edition)
        {
            var newSelectedEdition = Editions.First(x => x.Year == edition);
            Title = "TOP2000 - " + newSelectedEdition.Year;
            CanToggleGrouping = newSelectedEdition.HasPlayDateAndTime;
            if (!CanToggleGrouping && ListingGroup == ListingGroup.PlayTime)
            {
                ListingGroup = ListingGroup.Position;
            }

            SelectedEdition = newSelectedEdition;
            await LoadAllListingsAsync();
            
            if (position.HasValue)
            {
                SelectedItem = Listings
                    .OfType<TrackListingViewModel>()
                    .FirstOrDefault(x => x.Position == position.Value);
            }
        }
    }
    
    async partial void OnSelectedItemChanged(ITrackListingViewModel? value)
    {
        if (value is not null)
        {
            if (value is TrackListingViewModel trackListing)
            {
                var details = await _top2000Services.TrackDetailsAsync(trackListing.TrackId);
                var trackListings = details.Listings
                    .Select(x => new TrackDetailsListingViewModel
                    {
                        Edition = x.Edition,
                        Delta = x.Delta?.ToString() ?? "",
                        DeltaFontSize = TrackDetailsListingViewModel.ConvertDeltaFontSize(x),
                        DeltaSymbol = TrackDetailsListingViewModel.ConvertDeltaToSymbol(x),
                        DeltaSymbolColour =
                            new SolidColorBrush(TrackDetailsListingViewModel.ConvertDeltaSymbolColour(x)),
                        Position = x.Position,
                        Status = x.Status
                    })
                    .ToList();
                
                SelectedListing = new TrackDetailsViewModel
                {
                    ParentMainWindowViewModel = this,
                    Artist =  trackListing.Artist,
                    Title = trackListing.Title,
                    RecordedYear = details.RecordedYear,
                    Listings = trackListings,
                    SelectedListing = trackListings.First(x => x.Edition == SelectedEdition.Year),
                };
            }

            if (value is TrackListingPositionGroup group)
            {
                // toggle to group
            }
        }
    }
    
    public async Task InitialiseViewModelAsync()
    {
        Editions = await _top2000Services.AllEditionsAsync();
        SelectedEdition = Editions.First();
        Title = "TOP2000 - " + SelectedEdition.Year;

        await LoadAllListingsAsync();
        
        IsLoaded = true;
    }
    
    
    [RelayCommand]
    private void ToggleOrder()
    {
        ListingOrder = ListingOrder == ListingOrder.Ascending 
            ? ListingOrder.Descending 
            : ListingOrder.Ascending;
        
        UpdateListings();
    }
    
    [RelayCommand]
    private void ToggleGrouping()
    {
        ListingGroup = ListingGroup == ListingGroup.Position 
            ? ListingGroup.PlayTime 
            : ListingGroup.Position;

        UpdateListings();
    }

    partial void OnListingOrderChanged(ListingOrder value)
    {
        OrderIcon = value == ListingOrder.Ascending
            ? Symbols.Up
            : Symbols.Down;
    }
    
    partial void OnListingGroupChanged(ListingGroup value)
    {
        GroupIcon = value == ListingGroup.Position
            ? Symbols.ListingMenu
            : Symbols.Clock;
    }

    private async Task LoadAllListingsAsync()
    {
        _originalListings = await _top2000Services.AllListingsOfEditionAsync(SelectedEdition?.Year ?? 2025);
        Filters.UpdateCounters(_originalListings.CountBy(x => x.DeltaType));

        UpdateListings();
    }

    public void UpdateListings()
    {
        var newListings = _originalListings.Where(Filters.ShouldShow);

        newListings = ListingOrder == ListingOrder.Ascending
            ? newListings.OrderBy(x => x.Position)
            : newListings.OrderByDescending(x => x.Position);

        var filteredListing = newListings.ToList();
        if (filteredListing.Count > 100)
        {
            if (ListingGroup == ListingGroup.Position)
            {
                Listings = filteredListing
                    .GroupByPosition()
                    .SelectMany(TransformPositionGrouping)
                    .ToList();

                ListingGroups = Listings
                    .Where(x => x.IsHeader)
                    .ToList();
            }
            else
            {
                ListingGroups = Listings
                    .Where(x => x.IsHeader)
                    .ToList();
                
                Listings = filteredListing
                    .GroupByPlayUtcDateAndTime()
                    .SelectMany(TransformDateTimeGrouping)
                    .ToList();
            }
        }
        else
        {

            ListingGroups = [];
            Listings = filteredListing
                .Select(TransformTrackListingViewModel)
                .ToList();
        }
    }

    
    private static IEnumerable<ITrackListingViewModel> TransformDateTimeGrouping(IGrouping<DateTime, TrackListing> group)
    {
        return new[]
            {
                (ITrackListingViewModel)new TrackListingPlayTimeGroup { GroupName = MakeNiceDateTimeString(group.Key) }
            }
            .Concat(group.Select(TransformTrackListingViewModel));
    }
    
    private static string MakeNiceDateTimeString(DateTime dateTime)
    {
        var hourPlus = dateTime.ToLocalTime().AddHours(1);
        return $"{dateTime.ToLocalTime():dddd dd MMM HH:00}-{hourPlus:H:00}";
    }

    private static IEnumerable<ITrackListingViewModel> TransformPositionGrouping(IGrouping<string, TrackListing> group)
    {
        return new[] { (ITrackListingViewModel)new TrackListingPositionGroup { GroupName = group.Key } }.Concat(group.Select(TransformTrackListingViewModel));
    }
   
    private static ITrackListingViewModel TransformTrackListingViewModel(TrackListing x)
    {
        return new TrackListingViewModel
        {
            TrackId = x.TrackId,
            Artist = x.Artist,
            Title = x.Title,
            Delta = TrackListingViewModel.ConvertDeltaToString(x),
            DeltaFontSize = x.DeltaType.ToFontSize(),
            PositionString = x.Position.ToString(),
            Position = x.Position,
            DeltaSymbol = x.DeltaType.ToSymbol(),
            DeltaSymbolColour = x.DeltaType.ToBrush(),
            LocalPlayDateTime = x.PlayUtcDateAndTime.ToLocalTime()
        };
    }
}