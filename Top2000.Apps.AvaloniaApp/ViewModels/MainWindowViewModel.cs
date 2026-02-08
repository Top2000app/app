using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Top2000.Apps.AvaloniaApp.Assets;
using Top2000.Apps.AvaloniaApp.Views;
using Top2000.Features;
using Top2000.Features.Editions;
using Top2000.Features.Listings;
using Top2000.Features.TrackInformation;

namespace Top2000.Apps.AvaloniaApp.ViewModels;

public interface ICanFilterListings
{
    void UpdateListings();
}

public interface IHandleGroupSelection
{
    void SelectingGroup(ITrackListingViewModel group);
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


public partial class MainWindowViewModel : ViewModelBase, ICanFilterListings, IHandleGroupSelection
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
        if (SelectedEdition?.Year != edition)
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

    private IScrollListingListIntoView? _scrollService;

    [ObservableProperty] private List<ITrackListingViewModel> _groupTimeGroupListing = [];
    [ObservableProperty] private List<ITrackListingViewModel> _positionGroupListing = [];
    [ObservableProperty] private bool _showTimeGroupSelection = false;
    [ObservableProperty] private bool _showPositionGroupSelection = false;
    [ObservableProperty] private bool _showGroupSelection = false;

    public void SelectingGroup(ITrackListingViewModel group)
    {
        ShowTimeGroupSelection = false;
        ShowPositionGroupSelection = false;
        ShowGroupSelection = false;

        GroupTimeGroupListing = [];
        PositionGroupListing = [];
        SelectedItem = _selectUponGroupClosing;
        _scrollService?.ScrollIntoView(group);
    }

    private ITrackListingViewModel? _selectUponGroupClosing;
    async partial void OnSelectedItemChanged(ITrackListingViewModel? oldValue, ITrackListingViewModel? newValue)
    {
        if (newValue is TrackListingViewModel trackListing)
        {
            await DisplayTheSelectedListingAsync(trackListing);
        }

        if (newValue is TrackListingPositionGroup)
        {
            ShowGroupSelection = true;
            _selectUponGroupClosing = oldValue;
            ShowPositionGroupSelection = true;
            PositionGroupListing = Listings
                .Where(x => x is TrackListingPositionGroup)
                .Cast<TrackListingPositionGroup>()
                .Select(x => new TrackListingPosition { ItemText = x.GroupName, Parent = x })
                .Cast<ITrackListingViewModel>()
                .ToList();
        }

        if (newValue is TrackListingPlayDateTimeGroup)
        {
            ShowGroupSelection = true;
            _selectUponGroupClosing = oldValue;
            ShowTimeGroupSelection = true;
            GroupTimeGroupListing = Listings
                .Where(x => x is TrackListingPlayDateTimeGroup)
                .Cast<TrackListingPlayDateTimeGroup>()
                .OrderBy(x => x.PlayTime)
                .GroupBy(x => x.PlayTime.Date)
                .Select(TransformTrackListingPlayDateGroup)
                .ToList();
        }
    }

    private async Task DisplayTheSelectedListingAsync(TrackListingViewModel trackListing)
    {
        var details = await _top2000Services.TrackDetailsAsync(trackListing.TrackId);
        var trackListings = details.Listings
            .Select(Transform)
            .ToList();
                
        SelectedListing = new TrackDetailsViewModel
        {
            ParentMainWindowViewModel = this,
            Artist =  trackListing.Artist,
            Title = trackListing.Title,
            RecordedYear = details.RecordedYear,
            Listings = trackListings,
            SelectedListing = trackListings.First(x => x.Edition == SelectedEdition!.Year),
        };
    }
    

    private static TrackDetailsListingViewModel Transform(ListingInformation x)
    {
        return new TrackDetailsListingViewModel
        {
            Edition = x.Edition,
            Delta = x.Delta?.ToString() ?? "",
            DeltaFontSize = TrackDetailsListingViewModel.ConvertDeltaFontSize(x),
            DeltaSymbol = TrackDetailsListingViewModel.ConvertDeltaToSymbol(x),
            DeltaSymbolColour =
                new SolidColorBrush(TrackDetailsListingViewModel.ConvertDeltaSymbolColour(x)),
            Position = x.Position,
            Status = x.Status
        };
    }
    
    public async Task InitialiseViewModelAsync(IScrollListingListIntoView scrollService)
    {
        _scrollService = scrollService; 
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
        if (ListingGroup == ListingGroup.Position)
        {
            Listings = filteredListing
                .GroupByPosition(_originalListings.Count)
                .SelectMany(TransformPositionGrouping)
                .ToList();
        }
        else
        {
            Listings = filteredListing
                .GroupByPlayLocalDateAndTime()
                .SelectMany(TransformTrackListingPlayDateTimeGroup)
                .ToList();
        }
    }
    
    private static IEnumerable<ITrackListingViewModel> TransformTrackListingPlayDateTimeGroup(IGrouping<DateTime, TrackListing> group)
    {
        return new[]
            {
                new TrackListingPlayDateTimeGroup
                {
                    GroupName = TrackListingPlayDateTimeGroup.MakeNiceDateTimeString(group.Key),
                    PlayTime = group.Key
                }
            }
            .Concat(group.Select(TransformTrackListingViewModel));
    }


    private ITrackListingViewModel TransformTrackListingPlayDateGroup(IGrouping<DateTime, TrackListingPlayDateTimeGroup> group)
    {
        return new TrackListingPlayDateGroup
        {
            GroupName = group.Key.ToString("dddd dd MMM "),
            PlayTimes = group.Select(x => new TrackListingPlayTimeItem
            {
                NewGroupHandler = this,
                Parent = x,
                Time = $"{x.PlayTime:HH:00}-{x.PlayTime.AddHours(1):H:00}",
            }).ToList()
        };
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

