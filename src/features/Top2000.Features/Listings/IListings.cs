namespace Top2000.Features.Listings;

public interface IListings
{
    public Task<HashSet<TrackListing>> AllListingsOfEditionAsync(int edition, CancellationToken cancellationToken = default);
}