namespace Top2000.Features.SQLite.AllListingsOfEdition;

public class TrackCountHolder
{
    private readonly Dictionary<int, List<TrackCounter>> counters = [];

    public async Task<List<TrackCounter>> CountTrackCountForEditionAsync(SQLiteAsyncConnection connection, int edition)
    {
        if (!counters.TryGetValue(edition, out var value))
        {
            var sql = "SELECT TrackId, COUNT(TrackId) AS TrackCount " +
                "FROM Listing " +
                "WHERE Edition <= ? " +
                "GROUP BY TrackId";

            var trackCounters = await connection.QueryAsync<TrackCounter>(sql, edition);
            var list = trackCounters.Where(x => x.TrackCount > 1).ToList();
            value = list;
            counters.Add(edition, value);
        }

        return value;
    }
}
