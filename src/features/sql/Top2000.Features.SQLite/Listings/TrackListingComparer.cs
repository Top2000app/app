using Top2000.Features.Listing;

namespace Top2000.Features.SQLite.Listings;

public class TrackListingComparer : IEqualityComparer<TrackListing>
{
    public bool Equals(TrackListing? x, TrackListing? y) => x?.Position == y?.Position;

    public int GetHashCode(TrackListing obj) => obj.Position.GetHashCode();
}
