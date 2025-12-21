using Top2000.Data.JsonClientDatabase.Models;
using Top2000.Features.Listings;

namespace Top2000.Features.Json.Listings;

public class ListingFeature : IListings
{
    private readonly Top2000DataContext _data;

    public ListingFeature(Top2000DataContext data)
    {
        _data = data;
    }
    
    public Task<HashSet<TrackListing>> AllListingsOfEditionAsync(int edition, CancellationToken cancellationToken = default)
    {
        var currentListings = _data.Listings.Where(l => l.EditionId == edition);
        var previousListings = _data.Listings.Where(l => l.EditionId == edition - 1)
            .ToDictionary(l => l.TrackId, l => l.Position);
        
        var items = currentListings
            .Join(_data.Tracks, 
                listing => listing.TrackId, 
                track => track.Id, 
                (listing, track) => new TrackListing
                {
                    TrackId = listing.TrackId,
                    Position = listing.Position,
                    Delta = previousListings.TryGetValue(listing.TrackId, out var prevPos) 
                        ? prevPos - listing.Position 
                        : (int?)null,
                    PlayUtcDateAndTime = listing.PlayUtcDateAndTime ?? DateTime.MinValue,
                    Title = track.Title,
                    Artist = track.Artist,
                    IsRecurring = false
                })
            .OrderBy(x => x.Position)
            .ToList();

        var itemsWithNullDelta = items.Where(x => x.Delta is null);
        foreach (var item in itemsWithNullDelta)
        {
            var count = _data.Listings.Count(x => x.TrackId == item.TrackId);
            if (count > 1)
            {
                item.IsRecurring = true;
            }
        }

        return Task.FromResult(items.ToHashSet(new TrackListingComparer()));
    }
}