using Top2000.Features.AllListingsOfEdition;

namespace Top2000.Features.SQLite.AllListingsOfEdition;

public class TrackListingComparer : IEqualityComparer<TrackListing>
{
    public bool Equals(TrackListing? x, TrackListing? y) => x?.Position == y?.Position;

    public int GetHashCode(TrackListing obj) => obj.Position.GetHashCode();
}
