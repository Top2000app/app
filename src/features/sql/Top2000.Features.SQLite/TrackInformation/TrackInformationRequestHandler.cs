using Microsoft.Data.Sqlite;
using Top2000.Data.ClientDatabase.Models;
using Top2000.Features.TrackInformation;

namespace Top2000.Features.SQLite.TrackInformation;

public class TrackInformationFeature : ITrackInformation
{
    private readonly SqliteConnection _connection;

    public TrackInformationFeature(SqliteConnection connection)
    {
        this._connection = connection;
    }
    
    public async Task<TrackDetails> TrackDetailsAsync(int trackId, CancellationToken cancellationToken = default)
    {
        await _connection.OpenAsync(cancellationToken);

        var listings = new List<ListingInformation>();
        const string sql = "SELECT Year AS Edition, Position, PlayUtcDateAndTime " +
                           "FROM Edition LEFT JOIN Listing " +
                           "ON Listing.Edition = Edition.Year AND Listing.TrackId = $trackId";

        await using (var cmd = _connection.CreateCommand())
        {
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("$trackId", trackId);
            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                listings.Add(new ListingInformation
                {
                    Edition = reader.GetInt32(reader.GetOrdinal("Edition")),
                    Position = reader.IsDBNull(reader.GetOrdinal("Position")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("Position")),
                    PlayUtcDateAndTime = reader.IsDBNull(reader.GetOrdinal("PlayUtcDateAndTime")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("PlayUtcDateAndTime"))
                });
            }
        }

        Track track;
        await using (var cmd = _connection.CreateCommand())
        {
            cmd.CommandText = "SELECT Id, Title, Artist, RecordedYear FROM Track WHERE Id = $trackId";
            cmd.Parameters.AddWithValue("$trackId", trackId);
            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            if (!await reader.ReadAsync(cancellationToken))
            {
                throw new InvalidOperationException($"Track with Id {trackId} not found");
            }
            track = new Track
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Title = reader.GetString(reader.GetOrdinal("Title")),
                Artist = reader.GetString(reader.GetOrdinal("Artist")),
                RecordedYear = reader.GetInt32(reader.GetOrdinal("RecordedYear"))
            };
        }

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

        return new TrackDetails
        {
            Title = track.Title,
            Artist = track.Artist,
            RecordedYear = track.RecordedYear,
            Listings = new SortedSet<ListingInformation>(listings, new ListingInformationDescendingComparer()),
        };
    }
}