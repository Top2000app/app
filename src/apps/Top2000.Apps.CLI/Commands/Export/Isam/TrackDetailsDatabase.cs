using System.Collections.Immutable;
using System.Data;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Top2000.Apps.CLI.Database;
using Top2000.Features;

namespace DownloaderApp;

public sealed class Database
{
    private readonly Top2000DbContext _top2000DbContext;

    public Database(Top2000DbContext top2000DbContext)
    {
        _top2000DbContext = top2000DbContext;
    }
    
    public async Task<List<int>> AllTrackIdsAsync()
    {
        return await _top2000DbContext.Tracks
            .OrderBy(x => x.Id)
            .Select(x => x.Id)
            .ToListAsync();
    }

    public async Task<TrackDetails> TrackDetailsAsync(int trackId)
    {
        var listings = await _top2000DbContext.Editions
            .OrderBy(e => e.Year)
            .Select(edition => new
            {
                Year = edition.Year,
                Listing = _top2000DbContext.Listings
                    .FirstOrDefault(l => l.EditionId == edition.Year && l.TrackId == trackId)
            })
            .Select(x => new ListingInformation
            {
                Edition = x.Year,
                Position = x.Listing != null ? x.Listing.Position : null,
                PlayUtcDateAndTime = x.Listing != null ? x.Listing.PlayUtcDateAndTime : null
            })
            .ToListAsync();


        var track = await _top2000DbContext.Tracks
            .FirstAsync(x => x.Id == trackId); 

        var statusStrategy = new ListingStatusStrategy(track.RecordedYear);

        ListingInformation? previous = null;

        foreach (var listing in listings)
        {
            if (previous != null && previous.Position.HasValue)
            {
                listing.Offset = listing.Position - previous.Position;
            }

            listing.Status = statusStrategy.Determine(listing);
            previous = listing;
        }

        return new TrackDetails(track.Title, track.Artist, track.RecordedYear, listings.ToImmutableSortedSet(new ListingInformationDescendingComparer()));
    }
}