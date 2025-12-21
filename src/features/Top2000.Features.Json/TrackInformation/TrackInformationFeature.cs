using Top2000.Data.JsonClientDatabase.Models;
using Top2000.Features.TrackInformation;

namespace Top2000.Features.Json.TrackInformation;

public class TrackInformationFeature : ITrackInformation
{
    private readonly Top2000DataContext _data;

    public TrackInformationFeature(Top2000DataContext data)
    {
        _data = data;
    }
    
    public Task<TrackDetails> TrackDetailsAsync(int trackId, CancellationToken cancellationToken = default)
    {
        var track = _data.Tracks.FirstOrDefault(x => x.Id == trackId);
        if (track is null)
        {
            return Task.FromResult<TrackDetails>(null);
        }
        
        var listings = _data.Editions
            .GroupJoin(_data.Listings.Where(l => l.TrackId == trackId),
                edition => edition.Year,
                listing => listing.EditionId,
                (edition, listingGroup) => new
                {
                    Edition = edition.Year,
                    Listing = listingGroup.FirstOrDefault()
                    
                })
            .Select(x => new ListingInformation
            {
                Edition = x.Edition,
                Position = x.Listing?.Position,
                PlayUtcDateAndTime = x.Listing?.PlayUtcDateAndTime
            })
            .ToList();

        var statusStrategy = new ListingStatusStrategy(track.RecordedYear);
        ListingInformation? previous = null;
        foreach (var listing in listings.OrderBy(x => x.Edition))
        {
            if (previous?.Position != null && listing.Position.HasValue)
            {
                listing.Offset = listing.Position - previous.Position;
            }

            listing.Status = statusStrategy.Determine(listing);
            previous = listing;
        }
        
        var trackDetails = new TrackDetails
        {
            Title = track.Title,
            Artist = track.Artist,
            RecordedYear =  track.RecordedYear,
            Listings = new SortedSet<ListingInformation>(listings, new ListingInformationDescendingComparer())
        };
        
        return Task.FromResult(trackDetails);
    }
}