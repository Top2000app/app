namespace Top2000.Features.Listing;

public interface IListings
{
    public Task<HashSet<TrackListing>> AllListingsOfEditionAsync(int edition, CancellationToken cancellationToken = default);
}