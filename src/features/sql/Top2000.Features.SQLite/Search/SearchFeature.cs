using Top2000.Features.Searching;

namespace Top2000.Features.SQLite.Searching;

public class SearchFeature : ISearch
{
    private readonly SqliteConnection _connection;

    public SearchFeature(SqliteConnection connection)
    {
        this._connection = connection;
    }

    public async Task<List<IGrouping<string, SearchedTrack>>> SearchAsync(string queryString, int latestYear, ISort sorting, IGroup group,
        CancellationToken cancellationToken = default)
    {
        var results = new List<SearchedTrack>();

        if (!string.IsNullOrWhiteSpace(queryString))
        {
            await _connection.OpenAsync(cancellationToken);

            results = int.TryParse(queryString, out var year)
                ? await SearchOnYearAsync(queryString, latestYear, year, cancellationToken)
                : await SearchOnTitleAndArtistAsync(queryString, latestYear, cancellationToken);
        }

        var sorted = sorting.Sort(results);
        return group.Group(sorted).ToList();
    }

    private async Task<List<SearchedTrack>> SearchOnTitleAndArtistAsync(string queryString, int latestYear, CancellationToken cancellationToken)
    {
        List<SearchedTrack> results = [];
        const string sql = "SELECT Id, Title, Artist, RecordedYear, Listing.Position AS Position " +
                           "FROM Track " +
                           "LEFT JOIN Listing ON Track.Id = Listing.TrackId AND Listing.Edition = $edition " +
                           "WHERE (Title LIKE $like) OR (Artist LIKE $like) OR (SearchTitle LIKE $like) OR (SearchArtist LIKE $like) " +
                           "LIMIT 100";

        await using var cmd = _connection.CreateCommand();
        cmd.CommandText = sql;
        cmd.Parameters.AddWithValue("$edition", latestYear);
        cmd.Parameters.AddWithValue("$like", $"%{queryString}%");
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            results.Add(new SearchedTrack
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Title = reader.GetString(reader.GetOrdinal("Title")),
                Artist = reader.GetString(reader.GetOrdinal("Artist")),
                RecordedYear = reader.GetInt32(reader.GetOrdinal("RecordedYear")),
                Position = reader.IsDBNull(reader.GetOrdinal("Position")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("Position")),
                LatestEdition = latestYear,
            });
        }

        return results;
    }

    private async Task<List<SearchedTrack>> SearchOnYearAsync(string queryString, int latestYear, int year, CancellationToken cancellationToken)
    {
        List<SearchedTrack> results = [];
        const string sql = "SELECT Id, Title, Artist, RecordedYear, Listing.Position AS Position " +
                           "FROM Track " +
                           "LEFT JOIN Listing ON Track.Id = Listing.TrackId AND Listing.Edition = $edition " +
                           "WHERE RecordedYear = $year " +
                           "LIMIT 100";

        await using var cmd = _connection.CreateCommand();
        cmd.CommandText = sql;
        cmd.Parameters.AddWithValue("$edition", queryString);
        cmd.Parameters.AddWithValue("$year", year);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            results.Add(new SearchedTrack
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Title = reader.GetString(reader.GetOrdinal("Title")),
                Artist = reader.GetString(reader.GetOrdinal("Artist")),
                RecordedYear = reader.GetInt32(reader.GetOrdinal("RecordedYear")),
                Position = reader.IsDBNull(reader.GetOrdinal("Position")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("Position")),
                LatestEdition = latestYear,
            });
        }

        return results;
    }
}
