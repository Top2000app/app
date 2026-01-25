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
    private static readonly SolidColorBrush WhiteColourBrush = new (Colors.White);

    public required TrackListingDeltaType FilterBy { get; init; }
    
    public required string DisplayIcon { get; init; }

    public required SolidColorBrush DisplayColour { get; init; }

    public required ICanFilterListings Parent { get; init; }

    [ObservableProperty] private int _count = 0;
    
    [ObservableProperty]
    private bool _isChecked;

    [ObservableProperty] 
    private SolidColorBrush _displayColourBrush = new (Colors.White);
    
    partial void OnIsCheckedChanged(bool value)
    {
        Parent.UpdateListings();

        DisplayColourBrush = value
            ? WhiteColourBrush
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
    void UpdateListings();
}

public class FilterCollection : List<TrackListingDeltaFilterViewModel>
{
    private readonly Dictionary<TrackListingDeltaType, TrackListingDeltaFilterViewModel> _filters;

    private FilterCollection(Dictionary<TrackListingDeltaType, TrackListingDeltaFilterViewModel> filters)
    {
        _filters = filters;
    }
    
    public static FilterCollection Initialise(ICanFilterListings parent)
    {
        var filters = new Dictionary<TrackListingDeltaType, TrackListingDeltaFilterViewModel>
        {
            { TrackListingDeltaType.NoChange, CreateNew(TrackListingDeltaType.NoChange, parent) },
            { TrackListingDeltaType.Increased, CreateNew(TrackListingDeltaType.Increased, parent) },
            { TrackListingDeltaType.Decreased, CreateNew(TrackListingDeltaType.Decreased, parent) },
            { TrackListingDeltaType.New, CreateNew(TrackListingDeltaType.New, parent) },
            { TrackListingDeltaType.Recurring, CreateNew(TrackListingDeltaType.Increased, parent) }
        };
        
        return new FilterCollection(filters)
        {
            filters[TrackListingDeltaType.NoChange],
            filters[TrackListingDeltaType.Increased],
            filters[TrackListingDeltaType.Decreased],
            filters[TrackListingDeltaType.New],
            filters[TrackListingDeltaType.Recurring],
        };
    }
    
    
    private static TrackListingDeltaFilterViewModel CreateNew(TrackListingDeltaType type, ICanFilterListings parent)
    {
        return new TrackListingDeltaFilterViewModel
        {
            FilterBy = type,
            DisplayIcon = type.ToSymbol(),
            DisplayColour = type.ToBrush(),
            DisplayColourBrush = type.ToBrush(),
            Parent = parent,
        };
    }
    
    public void UpdateCounters(IEnumerable<KeyValuePair<TrackListingDeltaType, int>> newCounts)
    {
        foreach (var newCount in newCounts)
        {
            var filter = _filters[newCount.Key];
            filter.Count = newCount.Value;

            if (filter is { Count: 0, IsChecked: true })
            {
                filter.IsChecked = false;
            }
        }
    }

    public bool ShowAll() => this.All(x => !x.IsChecked);
    
    public bool ShouldShow(TrackListing listing) => ShowAll() || _filters[listing.DeltaType].IsChecked;
    
}


public partial class MainWindowViewModel : ViewModelBase, ICanFilterListings
{
    private SortedSet<Edition> Editions { get; set; }
    private HashSet<TrackListing> _originalListings = [];
    private List<TrackListing> _filteredListings = [];
    private ListingOrder _listingOrder = ListingOrder.Position;
    private readonly ITop2000Services _top2000Services;
    private bool _isLoadingListings = false;

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
    
    [ObservableProperty] 
    private List<ITrackListingViewModel> _listings = [];
    
    [ObservableProperty]
    private Edition _selectedEdition;
  
    [ObservableProperty]
    private string _title = "Top2000";

    [ObservableProperty]
    private bool _isLoaded;

    [ObservableProperty] 
    private TrackDetailsViewModel? _selectedListing;
    
    [ObservableProperty]
    private ITrackListingViewModel? _selectedItem;
    
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
    
    
    [RelayCommand]
    private void ShowByPosition()
    {
        _listingOrder = _listingOrder == ListingOrder.Position 
            ? ListingOrder.PositionDescending 
            : ListingOrder.Position;
        
        UpdateListings();
    }
    
    [RelayCommand]
    private void ShowByPlayTime()
    {
        _listingOrder = _listingOrder == ListingOrder.PlayTime 
            ? ListingOrder.PlayTimeDescending 
            : ListingOrder.PlayTime;

        UpdateListings();
    }

    private async Task LoadAllListingsAsync()
    {
        _isLoadingListings = true;
        
        _originalListings = await _top2000Services.AllListingsOfEditionAsync(this.SelectedEdition.Year);
        Filters.UpdateCounters(_originalListings.CountBy(x => x.DeltaType));

        UpdateListings();
            
        _isLoadingListings = false;
    }

    public void UpdateListings()
    {
        var newListings = _originalListings
                .Where(Filters.ShouldShow)
            ;

        switch (_listingOrder)
        {
            case ListingOrder.PositionDescending:
                newListings = newListings.OrderByDescending(y => y.Position);
                break;
            case ListingOrder.Position:
                newListings = newListings.OrderBy(y => y.Position);
                break;
            case ListingOrder.PlayTime:
                newListings = newListings.OrderBy(y => y.PlayUtcDateAndTime);
                break;
            case ListingOrder.PlayTimeDescending:
                newListings = newListings.OrderByDescending(y => y.PlayUtcDateAndTime);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        var filteredListing = newListings.ToList();
        if (filteredListing.Count > 100)
        {
            if (_listingOrder is ListingOrder.Position or ListingOrder.PositionDescending)
            {
                Listings = filteredListing
                    .GroupByPosition()
                    .SelectMany(TransformPositionGrouping)
                    .ToList();
            }
            else
            {
                Listings = filteredListing
                    .GroupByPlayUtcDateAndTime()
                    .SelectMany(TransformDateTimeGrouping)
                    .ToList();
            }
        }
        else
        {
            Listings = filteredListing
                .Select(TransformTrackListingViewModel)
                .ToList();
        }
    }

    
    private static IEnumerable<ITrackListingViewModel> TransformDateTimeGrouping(IGrouping<DateTime, TrackListing> group)
    {
        return new[]
            {
                (ITrackListingViewModel)new TrackListingViewModelGroup { GroupName = MakeNiceDateTimeString(group.Key) }
            }
            .Concat(group.Select(TransformTrackListingViewModel));
    }

    
    private static string MakeNiceDateTimeString(DateTime dateTime)
    {
        var hourPlus = dateTime.ToLocalTime().AddHours(1);
        return $"{dateTime.ToLocalTime():dddd dd MMM HH:00}-{hourPlus:H:00}";
    }

    private TrackListingDeltaFilterViewModel TransformToFilter(KeyValuePair<TrackListingDeltaType, int> x)
    {
        return new TrackListingDeltaFilterViewModel()
        {
            Count = x.Value,
            DisplayIcon = x.Key.ToSymbol(),
            DisplayColour = x.Key.ToBrush(),
            DisplayColourBrush = x.Key.ToBrush(),
            FilterBy = x.Key,
            IsChecked = false,
            Parent = this,
        };
    }

    private static IEnumerable<ITrackListingViewModel> TransformPositionGrouping(IGrouping<string, TrackListing> group)
    {
        return new[] { (ITrackListingViewModel)new TrackListingViewModelGroup { GroupName = group.Key } }.Concat(group.Select(TransformTrackListingViewModel));
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