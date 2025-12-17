using Top2000.Features.Editions;
using Top2000.Features.Listing;
using Top2000.Features.Searching;
using Top2000.Features.TrackInformation;

namespace Top2000.Features;

public class Top2000Services : IListings, ITrackInformation, IEditions, ISearch
{
    private readonly IListings _listings;
    private readonly ITrackInformation _trackInformation;
    private readonly IEditions _editions;
    private readonly ISearch _search;

    public Top2000Services(
        IListings listings,
        ITrackInformation trackInformation,
        IEditions editions,
        ISearch search)
    {
        _listings = listings;
        _trackInformation = trackInformation;
        _editions = editions;
        _search = search;
    }
    
    public Task<SortedSet<Edition>> AllEditionsAsync(CancellationToken cancellationToken = default)
        => _editions.AllEditionsAsync(cancellationToken);
    
    public Task<HashSet<TrackListing>> AllListingsOfEditionAsync(int edition, CancellationToken cancellationToken = default)
        => _listings.AllListingsOfEditionAsync(edition, cancellationToken);
    
    public Task<List<IGrouping<string, SearchedTrack>>> SearchAsync(string queryString, int latestYear, ISort sorting, IGroup group, CancellationToken cancellationToken = default)
        => _search.SearchAsync(queryString, latestYear, sorting, group, cancellationToken);
    
    public Task<TrackDetails> TrackDetailsAsync(int trackId, CancellationToken cancellationToken = default)
        => _trackInformation.TrackDetailsAsync(trackId, cancellationToken);
}