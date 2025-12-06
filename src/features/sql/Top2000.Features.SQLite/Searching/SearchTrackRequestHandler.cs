using System.Collections.ObjectModel;
using MoreLinq;
using Microsoft.Data.Sqlite;
using Top2000.Features.Searching;

namespace Top2000.Features.SQLite.Searching;

public class SearchTrackRequestHandler : IRequestHandler<SearchTrackRequest, List<IGrouping<string, SearchedTrack>>>
{
    private readonly SqliteConnection connection;

    public SearchTrackRequestHandler(SqliteConnection connection)
    {
        this.connection = connection;
    }

    public async Task<List<IGrouping<string, SearchedTrack>>> Handle(SearchTrackRequest request, CancellationToken cancellationToken)
    {
        var results = new List<SearchedTrack>();

        if (!string.IsNullOrWhiteSpace(request.QueryString))
        {
            await connection.OpenAsync(cancellationToken);

            if (int.TryParse(request.QueryString, out var year))
            {
                var sql = "SELECT Id, Title, Artist, RecordedYear, Listing.Position AS Position " +
                          "FROM Track " +
                          "LEFT JOIN Listing ON Track.Id = Listing.TrackId AND Listing.Edition = $edition " +
                          "WHERE RecordedYear = $year " +
                          "LIMIT 100";

                await using var cmd = connection.CreateCommand();
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("$edition", request.LatestYear);
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
                        LatestEdition = request.LatestYear,
                    });
                }
            }
            else
            {
                var like = $"%{request.QueryString}%";
                var sql = "SELECT Id, Title, Artist, RecordedYear, Listing.Position AS Position " +
                          "FROM Track " +
                          "LEFT JOIN Listing ON Track.Id = Listing.TrackId AND Listing.Edition = $edition " +
                          "WHERE (Title LIKE $like) OR (Artist LIKE $like) OR (SearchTitle LIKE $like) OR (SearchArtist LIKE $like) " +
                          "LIMIT 100";

                await using var cmd = connection.CreateCommand();
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("$edition", request.LatestYear);
                cmd.Parameters.AddWithValue("$like", like);
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
                        LatestEdition = request.LatestYear,
                    });
                }
            }
        }

        var sorted = request.Sorting.Sort(results);
        return request.Grouping.Group(sorted).ToList();
    }
}
