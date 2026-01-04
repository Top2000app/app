using Top2000.Data.JsonClientDatabase.Models;
using Top2000.Features.Json.Data;
using Top2000.Features.Listings;

namespace Top2000.Features.Json.Listings;

public class ListingFeature : IListings
{
    private readonly DataProvider _dataProvider;

    public ListingFeature(DataProvider dataProvider)
    {
        _dataProvider = dataProvider;
    }
    
    public Task<HashSet<TrackListing>> AllListingsOfEditionAsync(int edition, CancellationToken cancellationToken = default)
    {
        var listings = _dataProvider.Value.ListingsOfEdition[edition];
        var returnSet = listings.Select(x => new TrackListing
            {
                DeltaType = (TrackListingDeltaType)x.DeltaType,
                Position = x.Position,
                TrackId = x.TrackId,
                Delta = x.Delta,
                PlayUtcDateAndTime = x.PlayUtcDateAndTime ?? DateTime.MinValue,
                Title = x.Title,
                Artist = x.Artist,
            })
            .ToHashSet(new TrackListingComparer());
        
        return Task.FromResult(returnSet);
    }
}