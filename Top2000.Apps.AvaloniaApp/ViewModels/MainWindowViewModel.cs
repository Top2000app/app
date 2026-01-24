using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Top2000.Apps.AvaloniaApp.Assets;
using Top2000.Features;
using Top2000.Features.Editions;
using Top2000.Features.Listings;
namespace Top2000.Apps.AvaloniaApp.ViewModels;

public enum ListingOrder
{
    Position,
    PositionDescending,
    PlayTime,
    PlayTimeDescending
}

public partial class TrackListingDeltaFilterViewModel : ViewModelBase
{
    public required TrackListingDeltaType FilterBy { get; init; }

    public required string DisplayIcon { get; init; }
    
    public required SolidColorBrush DisplayColour { get; init; }

    public required ICanFilterListings Parent { get; init; }
    
    public int Count { get; set; }
    
    [ObservableProperty]
    private bool _isChecked;

    [ObservableProperty] 
    private SolidColorBrush _displayColourBrush = new (Colors.White);

    partial void OnIsCheckedChanged(bool value)
    {
        Parent.FilterListingList();

        DisplayColourBrush = value
            ? new SolidColorBrush(Colors.White)
            : DisplayColour;
    }
}

public static class SymbolsExtensions
{
    extension(TrackListingDeltaType type)
    {
        public string ToSymbol()
        {
            return type switch
            {
                TrackListingDeltaType.NoChange => Symbols.Same,
                TrackListingDeltaType.Increased => Symbols.Up,
                TrackListingDeltaType.Decreased => Symbols.Down,
                TrackListingDeltaType.New => Symbols.Flag,
                TrackListingDeltaType.Recurring => Symbols.BackInList,
                _ => Symbols.Same
            };
        }

        public SolidColorBrush ToBrush()
        {
            var colour = type switch
            {

                TrackListingDeltaType.NoChange => Colours.GreyColour,
                TrackListingDeltaType.Increased => Colours.GreenColour,
                TrackListingDeltaType.Decreased => Colours.RedColour,
                TrackListingDeltaType.New => Colours.YellowColour,
                TrackListingDeltaType.Recurring => Colours.YellowColour,
                _ => Colours.GreyColour
            };
            
            return new SolidColorBrush(colour);
        }
    }
}

public interface ICanFilterListings
{
    void FilterListingList();
}

public partial class MainWindowViewModel : ViewModelBase, ICanFilterListings
{
    private ListingOrder _listingOrder = ListingOrder.Position;
    private readonly ITop2000Services _top2000Services;

    public MainWindowViewModel()
    {
        _top2000Services = new MockupTop2000Services();
    }
    
    public MainWindowViewModel(ITop2000Services services)
    {
        _top2000Services = services;
    }

    [ObservableProperty] private List<TrackListingDeltaFilterViewModel> _filters = [];

    [ObservableProperty] 
    private List<ITrackListingViewModel> listings = [];
    
    [ObservableProperty]
    private Edition selectedEdition;
  
    [ObservableProperty]
    private string title = "Top2000";

    [ObservableProperty]
    private bool isLoaded;

    [ObservableProperty] 
    private TrackDetailsViewModel? selectedListing;
    
    [ObservableProperty]
    public ITrackListingViewModel? selectedItem;
    
    public async Task ChangeSelectedEditionAsync(int edition, int? position)
    {
        if (SelectedEdition.Year != edition)
        {
            SelectedEdition = Editions.First(x => x.Year == edition);
            Title = "TOP2000 - " + SelectedEdition.Year;
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

            if (value is TrackListingViewModelGroup group)
            {
                Title = group.GroupName;
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

    private SortedSet<Edition> Editions { get; set; }

    [RelayCommand]
    private void ShowByPosition()
    {
        _listingOrder = _listingOrder == ListingOrder.Position 
            ? ListingOrder.PositionDescending 
            : ListingOrder.Position;
        
        ReOrder();
    }

    
    [RelayCommand]
    private void ShowByPlayTime()
    {
        _listingOrder = _listingOrder == ListingOrder.PlayTime 
            ? ListingOrder.PlayTimeDescending 
            : ListingOrder.PlayTime;

        ReOrder();
    }

    private void ReOrder()
    {
        IOrderedEnumerable<TrackListing>? newOrder = null;
        if (_listingOrder is ListingOrder.PositionDescending or ListingOrder.PlayTimeDescending)
        {
            newOrder = _filteredListings
                .OrderByDescending(x => x.Position);
              
        }
        else
        {
            newOrder = _filteredListings
                .OrderBy(x => x.Position);
        }
        
        if (_listingOrder is ListingOrder.Position or ListingOrder.PositionDescending)
        {
            Listings = newOrder
                .GroupByPosition()
                .SelectMany(group =>
                    new[] { (ITrackListingViewModel)new TrackListingViewModelGroup { GroupName = group.Key } }
                        .Concat(group.Select(TransformTrackListingViewModel)))
                .ToList();
        }
        else
        {
            Listings = newOrder
                .GroupByPlayUtcDateAndTime()
                .SelectMany(group => 
                    new[] { (ITrackListingViewModel)new TrackListingViewModelGroup { GroupName = MakeNiceDateTimeString(group.Key) } }
                        .Concat(group.Select(TransformTrackListingViewModel)))
                .ToList();
        }
    }

    private static string MakeNiceDateTimeString(DateTime dateTime)
    {
        var hourPlus = dateTime.ToLocalTime().AddHours(1);

        return $"{dateTime.ToLocalTime():dddd dd MMM HH:00}-{hourPlus:H:00}";
    }
    
    public void FilterListingList()
    {
        if (!_isLoadingListings)
        {
            var showAll = Filters.All(x => !x.IsChecked);

            if (showAll)
            {
                _filteredListings = _originalListings.ToList();
                Listings = _filteredListings
                    .GroupByPosition()
                    .SelectMany(group =>
                        new[] { (ITrackListingViewModel)new TrackListingViewModelGroup { GroupName = group.Key } }
                            .Concat(group.Select(TransformTrackListingViewModel)))
                    .ToList();
            }
            else
            {
                var toShow = Filters
                    .Where(x => x.IsChecked)
                    .Select(x => x.FilterBy)
                    .ToList();

                _filteredListings = _originalListings
                    .Where(x => toShow.Contains(x.DeltaType))
                    .ToList();

                if (_filteredListings.Count > 100)
                {
                    Listings = _filteredListings
                        .GroupByPosition()
                        .SelectMany(group => 
                            new[] { (ITrackListingViewModel)new TrackListingViewModelGroup { GroupName = group.Key } }
                                .Concat(group.Select(TransformTrackListingViewModel)))
                        .ToList();
                }
                else
                {
                    Listings = _filteredListings
                        .Select(TransformTrackListingViewModel)
                        .ToList();
                }
            }
        }
    }
    
    private bool _isLoadingListings = false;

    private HashSet<TrackListing> _originalListings = [];
    private List<TrackListing> _filteredListings = [];
    
    private async Task LoadAllListingsAsync()
    {
        _isLoadingListings = true;
        _originalListings = await _top2000Services.AllListingsOfEditionAsync(this.SelectedEdition.Year);
        _filteredListings = _originalListings.ToList();
        
        Filters = _originalListings
            
            .CountBy(x => x.DeltaType)
            .Select(x => new TrackListingDeltaFilterViewModel()
            {
                Count = x.Value,
                DisplayIcon = x.Key.ToSymbol(),
                DisplayColour = x.Key.ToBrush(),
                DisplayColourBrush = x.Key.ToBrush(),
                FilterBy = x.Key,
                IsChecked = false,
                Parent = this,
            })
            .OrderBy(x => x.FilterBy)
            .ToList();
        
        Listings = _originalListings
            .GroupByPosition()
            .SelectMany(group => 
                new[] { (ITrackListingViewModel)new TrackListingViewModelGroup { GroupName = group.Key } }
                .Concat(group.Select(TransformTrackListingViewModel)))
            .ToList();

        _isLoadingListings = false;
    }
    
   
    private static ITrackListingViewModel TransformTrackListingViewModel(TrackListing x)
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
            DeltaSymbol = x.DeltaType.ToSymbol(),
            DeltaSymbolColour = x.DeltaType.ToBrush(),
            LocalPlayDateTime = x.PlayUtcDateAndTime.ToLocalTime()
        };
    }
}