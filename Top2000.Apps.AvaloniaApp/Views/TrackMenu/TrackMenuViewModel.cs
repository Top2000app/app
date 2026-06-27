using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using Top2000.Apps.AvaloniaApp.Assets;
using Top2000.Apps.AvaloniaApp.ViewModels;
using Top2000.Apps.AvaloniaApp.Views.SelectedEdition;
using Top2000.Apps.AvaloniaApp.Views.Shell;
using Top2000.Features;
using Top2000.Features.Editions;
using Top2000.Features.Listings;
using Top2000.Features.Searching;
using Top2000.Features.TrackInformation;

namespace Top2000.Apps.AvaloniaApp.Views.TrackMenu;

public class DesignTrackMenuViewModel : TrackMenuViewModel
{
    public DesignTrackMenuViewModel() : base(new MockupTop2000Services())
    {
        Filters = FilterCollection.Initialise(this);
    }
}

public partial class TrackMenuViewModel : ObservableObject, ICanFilterListings, IHandleGroupSelection
{
    private SortedSet<Edition> Editions { get; set; } = [];
    private HashSet<TrackListing> _originalListings = [];
    private readonly ITop2000Services _top2000Services;
    private ITrackListingViewModel? _selectUponGroupClosing;
    private IShell? _shell;
   

    public TrackMenuViewModel(ITop2000Services services)
    {
        _top2000Services = services;
        Filters = FilterCollection.Initialise(this);
    }
    
    public FilterCollection Filters { get; set; }
    
    [ObservableProperty] private List<ITrackListingViewModel> _listings = [];
    [ObservableProperty] private SelectedEditionViewModel? _selectedEdition;
    
    [ObservableProperty]
    public partial List<SearchTrackResultViewModel> SearchListing { get; set; }
    
    [ObservableProperty]
    public partial ITrackListingViewModel? SelectedItem { get; set; }

    [ObservableProperty]
    public partial string OrderIcon { get; set; } = Symbols.Up;
    
    [ObservableProperty]
    public partial string GroupIcon { get; set; } = Symbols.Clock;

    [ObservableProperty]
    public partial bool CanToggleGrouping { get; set; } = true;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsListingSelected))]
    public partial TrackDetailsViewModel? SelectedListing { get; set; }

    [ObservableProperty] public partial bool ShowSearchResults { get; set; } = false;
    [ObservableProperty] public partial string SearchString { get; set; }
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(GroupIcon))]
    public partial ListingsViewModel SelectedListingsViewModel { get; set; } = new ListingGroupedByPosition();

    public bool IsListingSelected => SelectedListing != null;
    
    public async Task ChangeSelectedEditionAsync(int edition, int? position)
    {
        if (SelectedEdition?.Year != edition)
        {
            var newSelectedEdition = Editions.First(x => x.Year == edition);
            _shell?.Title = "TOP2000 - " + newSelectedEdition.Year;
            CanToggleGrouping = newSelectedEdition.HasPlayDateAndTime;
            if (!CanToggleGrouping && SelectedListingsViewModel is ListingGroupByPlayTime)
            {
                SelectedListingsViewModel = SelectedListingsViewModel.ToggleGrouping();
            }

            SelectedEdition = new SelectedEditionViewModel
            {
                Year = newSelectedEdition.Year
            };
            
            await LoadAllListingsAsync();
            
            if (position.HasValue)
            {
                SelectedItem = Listings
                    .OfType<TrackListingViewModel>()
                    .FirstOrDefault(x => x.Position == position.Value);

                if (SelectedItem is not null)
                {
                    _scrollService!.ScrollIntoView(SelectedItem);
                }
            }
        }
    }

    private IScrollListingListIntoView? _scrollService;

    [ObservableProperty]
    public partial List<ITrackListingViewModel> GroupTimeGroupListing { get; set; } = [];

    [ObservableProperty]
    public partial List<ITrackListingViewModel> PositionGroupListing { get; set; } = [];

    [ObservableProperty]
    public partial bool ShowTimeGroupSelection { get; set; }

    [ObservableProperty]
    public partial bool ShowPositionGroupSelection { get; set; }

    [ObservableProperty]
    public partial bool ShowGroupSelection { get; set; }

    [ObservableProperty]
    public partial bool ShowSearchBar { get; set; }

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

    private CancellationTokenSource? _debounceCts;
    private readonly TimeSpan _debounceDelay = TimeSpan.FromMilliseconds(300);


    async partial void OnSearchStringChanged(string value)
    {
        _debounceCts?.Cancel();
        _debounceCts?.Dispose();

        _debounceCts = new CancellationTokenSource();
        var token = _debounceCts.Token;

        try
        {
            await Task.Delay(_debounceDelay, token);

            await RunSearchAsync(value, token);
        }
        catch (TaskCanceledException)
        {
            // swallow
        }
    }
    
    private async Task RunSearchAsync(string value, CancellationToken token)
    {
        try
        {
            var result = await _top2000Services.SearchAsync(
                value,
                Editions.First().Year,
                new SortByTitle(),
                new GroupByNothing(),
                token);

            if (!token.IsCancellationRequested && result.Any()) 
            {
                SearchListing = result.First()
                    .Select(x => new SearchTrackResultViewModel
                    {
                        Artist = x.Artist,
                        TitleWithRecordedYear = $"{x.Title} ({x.RecordedYear})",
                        TrackId = x.Id,
                        PositionInLatestEdition = x.PositionInLatestEdition
                    })
                    .ToList();

                ShowSearchResults = true;
            }
        }
        catch (TaskCanceledException)
        {
            // swallow
        }
    }

    partial void OnShowSearchBarChanged(bool value)
    {
        SearchString = string.Empty;

        if (!value)
        {
            ShowSearchResults = false;
        }
    }

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
                .OrderBy(x => int.Parse(x.GroupName.Split(' ')[0]))
                .GroupBy(x => int.Parse(x.GroupName.Split(' ')[0]) / 1000)
                .Select(TransformTrackListingPositionGroupHeader)
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
    
    private ITrackListingViewModel TransformTrackListingPositionGroupHeader(IGrouping<int, TrackListingPositionGroup> group)
    {
        var positionGroups = group.Select(x => new TrackListingPosition
        {
            ItemText = $"{x.PositionRangStart} - {x.PositionRangEnd}",
            NewGroupHandler = this,
            Parent = x,
        }).ToList();
        
        return new TrackListingPositionGroupHeader
        {
            ItemText = $"{positionGroups[0].Parent.PositionRangStart} - {positionGroups[^1].Parent.PositionRangEnd}",
            PositionGroups = positionGroups
        };
    }

    private async Task DisplayTheSelectedListingAsync(TrackListingViewModel trackListing)
    {
        var details = await _top2000Services.TrackDetailsAsync(trackListing.TrackId);
        var trackListings = details.Listings
            .Select(Transform)
            .ToList();
        
        var orderedListings = trackListings
            .Where(x => x.Position.HasValue)
            .OrderBy(x => x.Position)
            .ThenBy(x => x.Edition)
            .ToList();

        var trackListingsForGraph = trackListings
            .OrderBy(x => x.Edition)
            .Select(x => new ObservablePoint
            {
               X = x.Edition,
               Y = x.Position,
            })
            .ToArray();

        var xxx = trackListingsForGraph.Select(x => (int?)x.Y).ToList();

        var zoomRange = TrackDetailsViewModel.GetZoomRange(xxx);
        
        if (SelectedListing is null)
        {
            
            SelectedListing = new TrackDetailsViewModel
            {
                ParentMainWindowViewModel = this,
                Artist =  trackListing.Artist,
                Title = trackListing.Title,
                RecordedYear = details.RecordedYear,
                Listings = trackListings,
                SelectedListing = trackListings.First(x => x.Edition == SelectedEdition!.Year),
                LatestEditionPosition = trackListings.Last(x => x.Position.HasValue)?.Position.ToString() ?? "-",
                Highest = orderedListings.Last(),
                Lowest = orderedListings.First(),
                YMax = zoomRange.max,
                YMin = zoomRange.min,
              //  Positions = trackListingsForGraph.ToArray(),
                Series =
                [
                    new LineSeries<ObservablePoint>
                    {
                        Values = trackListingsForGraph,
                        GeometrySize = 6
                    }
                ]
            };
        }

        else
        {
            SelectedListing.YMax = zoomRange.max;
            SelectedListing.YMin = zoomRange.min;
            SelectedListing.Series =
            [
                new LineSeries<ObservablePoint>
                {
                    Values = trackListingsForGraph,
                    GeometrySize = 6
                }
            ];
        }
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
    
    public async Task InitialiseViewModelAsync(IScrollListingListIntoView scrollService, IShell shell)
    {
        _shell = shell;
        _scrollService = scrollService; 
        Editions = await _top2000Services.AllEditionsAsync();
        SelectedEdition = new SelectedEditionViewModel { Year = Editions.First().Year };
        shell.Title = "TOP2000 - " + SelectedEdition.Year;

        await LoadAllListingsAsync();
    }
    
    
    [RelayCommand]
    private void ToggleOrder()
    {
        SelectedListingsViewModel.ToggleOrder();
        OrderIcon = SelectedListingsViewModel.NextOrderSymbol;
        UpdateListings();
    }
    
    [RelayCommand]
    private void ToggleGrouping()
    {
        SelectedListingsViewModel = SelectedListingsViewModel.ToggleGrouping();
        GroupIcon = SelectedListingsViewModel.NextGroupSymbol;
        UpdateListings();
    }

    private async Task LoadAllListingsAsync()
    {
        _originalListings = await _top2000Services.AllListingsOfEditionAsync(SelectedEdition?.Year ?? 2025);
        Filters.UpdateCounters(_originalListings.CountBy(x => x.DeltaType));

        UpdateListings();
    }

    public void UpdateListings()
    {
        var newListings = _originalListings
            .Where(Filters.ShouldShow);
        
        Listings = SelectedListingsViewModel.CreateListing(newListings, _originalListings.Count);
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
}
