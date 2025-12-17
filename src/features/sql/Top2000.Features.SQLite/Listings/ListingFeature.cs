using Microsoft.Extensions.Options;
using Top2000.Data.ClientDatabase;
using Top2000.Features.Listing;

namespace Top2000.Features.SQLite.Listings;

public class ListingFeature : IListings
{
    private readonly SqliteConnection _connection;
    private readonly TrackCountHolder _trackCountHolder;

    public ListingFeature(SqliteConnection connection, TrackCountHolder trackCountHolder)
    {
        _connection = connection;
        _trackCountHolder = trackCountHolder;
    }
    
    public async Task<HashSet<TrackListing>> AllListingsOfEditionAsync(int edition, CancellationToken cancellationToken = default)
    {
        var counters = await _trackCountHolder.CountTrackCountForEditionAsync(_connection, edition);
        await _connection.OpenAsync(cancellationToken);
        const string sql = "SELECT Listing.TrackId, Listing.Position, (p.Position - Listing.Position) AS Delta, Listing.PlayUtcDateAndTime, Title, Artist " +
                           "FROM Listing JOIN Track ON Listing.TrackId = Id " +
                           "LEFT JOIN Listing as p ON p.TrackId = Id AND p.Edition = $prevEdition " +
                           "WHERE Listing.Edition = $edition " +
                           "ORDER BY Listing.Position";

        var items = new List<TrackListing>();
        await using var cmd = _connection.CreateCommand();
        cmd.CommandText = sql;
        cmd.Parameters.AddWithValue("$prevEdition", edition - 1);
        cmd.Parameters.AddWithValue("$edition", edition);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var trackListing = new TrackListing
            {
                TrackId = reader.GetInt32(reader.GetOrdinal("TrackId")),
                Position = reader.GetInt32(reader.GetOrdinal("Position")),
                Delta = reader.IsDBNull(reader.GetOrdinal("Delta")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("Delta")),
                PlayUtcDateAndTime = reader.IsDBNull(reader.GetOrdinal("PlayUtcDateAndTime")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("PlayUtcDateAndTime")),
                Title = reader.GetString(reader.GetOrdinal("Title")),
                Artist = reader.GetString(reader.GetOrdinal("Artist")),
                IsRecurring = false,
            };
            items.Add(trackListing);
        }

        var itemWithNullDelta = items.Where(x => x.Delta is null);
        foreach (var item in itemWithNullDelta)
        {
            var inCounters = counters.Find(x => x.TrackId == item.TrackId);
            if (inCounters?.TrackCount > 1)
            {
                item.IsRecurring = true;
            }
        }

        return items.ToHashSet(new TrackListingComparer());
    }
}
