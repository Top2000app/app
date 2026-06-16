using Top2000.Apps.AvaloniaApp.ViewModels;
using Top2000.Features.Listings;

namespace Top2000.Apps.AvaloniaApp.Views.TrackMenu;

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
            { TrackListingDeltaType.Recurring, CreateNew(TrackListingDeltaType.Recurring, parent) }
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
        var dictionary = newCounts.ToDictionary(x => x.Key , x => x.Value);
        foreach (var filter in this)
        {
            filter.Count = dictionary.GetValueOrDefault(filter.FilterBy, 0);
            if (filter is { Count: 0, IsChecked: true })
            {
                filter.IsChecked = false;
            }
        }
    }

    private bool ShowAll() => this.All(x => !x.IsChecked);
    
    public bool ShouldShow(TrackListing listing) => ShowAll() || _filters[listing.DeltaType].IsChecked;
    
}