using Top2000.Apps.AvaloniaApp.Assets;
using Top2000.Features.Listings;

namespace Top2000.Apps.AvaloniaApp.Views.TrackMenu;

public class ListingGroupedByPosition : ListingsViewModel
{
    public override ListingsViewModel ToggleGrouping() => new ListingGroupByPlayTime
    {
        IsOrderAscending = IsOrderAscending
    };

    public override string NextGroupSymbol => Symbols.Clock;

    protected override IEnumerable<ITrackListingViewModel> GroupListing(IOrderedEnumerable<TrackListing> listings, int originalListingCount)
    {
        return listings
            .GroupByPosition(originalListingCount)
            .SelectMany(Transform);

        static IEnumerable<ITrackListingViewModel> Transform(IGrouping<string, TrackListing> group)
        {
            var positionRange = group.Key.Split('-').Select(x => int.Parse(x.Trim())).ToArray();

            return new[]
            {
                new TrackListingPositionGroup
                {
                    PositionRangStart = positionRange[0],
                    PositionRangEnd = positionRange[1],
                    GroupName = group.Key
                }
            }.Concat(group.Select(TransformTrackListingViewModel));
        }
    }

}