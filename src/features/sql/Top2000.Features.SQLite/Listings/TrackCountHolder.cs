using Microsoft.Data.Sqlite;

namespace Top2000.Features.SQLite.Listings;

public class TrackCountHolder
{
    private readonly Dictionary<int, List<TrackCounter>> _counters = [];

    public async Task<List<TrackCounter>> CountTrackCountForEditionAsync(SqliteConnection connection, int edition)
    {
        if (!_counters.TryGetValue(edition, out var value))
        {
            await connection.OpenAsync();
            var sql = "SELECT TrackId, COUNT(TrackId) AS TrackCount " +
                      "FROM Listing " +
                      "WHERE Edition <= $edition " +
                      "GROUP BY TrackId";

            var list = new List<TrackCounter>();
            await using var cmd = connection.CreateCommand();
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("$edition", edition);
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var trackId = reader.GetInt32(reader.GetOrdinal("TrackId"));
                var trackCount = reader.GetInt32(reader.GetOrdinal("TrackCount"));
                if (trackCount > 1)
                {
                    list.Add(new TrackCounter { TrackId = trackId, TrackCount = trackCount });
                }
            }
            value = list;
            _counters.Add(edition, value);
        }

        return value;
    }
}
