using Top2000.Data.ClientDatabase.Models;
using Top2000.Features.TrackInformation;

namespace Top2000.Features.SQLite.TrackInformation;

public class TrackInformationRequestHandler : IRequestHandler<TrackInformationRequest, TrackDetails>
{
    private readonly SQLiteAsyncConnection connection;

    public TrackInformationRequestHandler(SQLiteAsyncConnection connection)
    {
        this.connection = connection;
    }

    public async Task<TrackDetails> Handle(TrackInformationRequest request, CancellationToken cancellationToken)
    {
        var sql =
            "SELECT Year AS Edition, Position, PlayUtcDateAndTime " +
            "FROM Edition LEFT JOIN Listing " +
            "ON Listing.Edition = Edition.Year AND Listing.TrackId = ?";

        var listings = (await connection.QueryAsync<ListingInformation>(sql, request.TrackId).ConfigureAwait(false))
            .OrderBy(x => x.Edition)
            .ToArray();

        var track = await connection.GetAsync<Track>(request.TrackId).ConfigureAwait(false);

        var statusStrategy = new ListingStatusStrategy(track.RecordedYear);

        ListingInformation? previous = null;

        foreach (var listing in listings)
        {
            if (previous is not null && previous.Position.HasValue)
            {
                listing.Offset = listing.Position - previous.Position;
            }

                listing.Status = statusStrategy.Determine(listing);
            previous = listing;
        }

        return new TrackDetails
        {
            Title = track.Title,
            Artist = track.Artist,
            RecordedYear = track.RecordedYear,
            Listings = new SortedSet<ListingInformation>(listings, new ListingInformationDescendingComparer()),
        };
    }
}
