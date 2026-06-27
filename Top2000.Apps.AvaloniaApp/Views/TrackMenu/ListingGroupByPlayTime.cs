using Top2000.Apps.AvaloniaApp.Assets;
using Top2000.Features.Listings;

namespace Top2000.Apps.AvaloniaApp.Views.TrackMenu;

public  class ListingGroupByPlayTime : ListingsViewModel
{

    public override ListingsViewModel ToggleGrouping() => new ListingGroupedByPosition
    {
        IsOrderAscending = IsOrderAscending
    };

    public override string NextGroupSymbol => Symbols.List;

    protected override IEnumerable<ITrackListingViewModel> GroupListing(IOrderedEnumerable<TrackListing> listings, int _)
    {
        return listings
            .GroupByPlayLocalDateAndTime()
            .SelectMany(Transform);
        
        static IEnumerable<ITrackListingViewModel> Transform(IGrouping<DateTime, TrackListing> group)
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
    }
    
}