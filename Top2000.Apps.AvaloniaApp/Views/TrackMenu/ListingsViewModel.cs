using CommunityToolkit.Mvvm.ComponentModel;
using Top2000.Apps.AvaloniaApp.Assets;
using Top2000.Apps.AvaloniaApp.ViewModels;
using Top2000.Features.Listings;

namespace Top2000.Apps.AvaloniaApp.Views.TrackMenu;

public abstract partial class ListingsViewModel : ObservableObject
{
    public abstract ListingsViewModel ToggleGrouping();

    public bool IsOrderAscending { get; set; } = true;

    public abstract string NextGroupSymbol { get; }

    public string NextOrderSymbol { get; private set; } = Symbols.Down;
    
    public void ToggleOrder()
    {
        IsOrderAscending = !IsOrderAscending;
        NextOrderSymbol = IsOrderAscending
            ? Symbols.Down
            : Symbols.Up;
    }

    public List<ITrackListingViewModel> CreateListing(IEnumerable<TrackListing> listings, int originalListingCount)
    {
        var orderedListings = IsOrderAscending
            ? listings.OrderBy(x => x.Position)
            : listings.OrderByDescending(x => x.Position);

        return GroupListing(orderedListings, originalListingCount)
            .ToList();
    }

    protected abstract IEnumerable<ITrackListingViewModel> GroupListing(IOrderedEnumerable<TrackListing> listings, int originalListingCount);
    
    protected static ITrackListingViewModel TransformTrackListingViewModel(TrackListing x)
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