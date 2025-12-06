using Microsoft.Data.Sqlite;

var services = new ServiceCollection()
    .AddLogging(configure => configure.AddConsole())
    .AddTop2000(builder =>
        {
            builder.DatabaseName("Top2000v2");
        })
    .BuildServiceProvider()
    ;

var assemblySource = services.GetRequiredService<Top2000AssemblyDataSource>();
var update = services.GetRequiredService<IUpdateClientDatabase>();
await update.RunAsync(assemblySource);

var connection = services.GetRequiredService<SqliteConnection>();
await connection.OpenAsync();

const string allTrackSql = "SELECT * FROM Track";
const string allEditionsSql = "SELECT * FROM Edition";
const string allListingSql = "SELECT * FROM Listing";

var allTracks = await ReadTracksAsync(connection, allTrackSql);
var allListings = await ReadListingsAsync(connection, allListingSql);
var allEditions = await ReadEditionYearsAsync(connection, allEditionsSql);

var lastYear = allEditions[^1];

var lines = new List<string>
{
    $"Id;Title;Artist;SearchTitle;SearchArtist;RecordedYear;LastPlayTimeUtc;{string.Join(';', allEditions)}"
};

foreach (var track in allTracks)
{
    var trackListings = allListings
        .Where(x => x.TrackId == track.Id)
        .ToList();

    var line = new StringBuilder($"{track.Id};{track.Title};{track.Artist};{track.SearchTitle ?? ""};{track.SearchArtist ?? ""};{track.RecordedYear};");

    var lastPlayUtcDateAndTime = trackListings.Find(x => x.Edition == lastYear)?.PlayUtcDateAndTime;
    line.Append(lastPlayUtcDateAndTime?.ToString("dd-MM-yyyy HH:mm:ss'Z'") ?? string.Empty);

    foreach (var edition in allEditions)
    {
        var listing = trackListings.Find(x => x.Edition == edition);
        var position = listing?.Position.ToString() ?? "";
        line.Append($";{position}");
    }

    lines.Add(line.ToString());

}

await File.WriteAllLinesAsync("top2000.csv", lines, encoding: Encoding.UTF8);
return;

static async Task<List<int>> ReadEditionYearsAsync(SqliteConnection conn, string sql)
{
    var years = new List<int>();
    await using var cmd = conn.CreateCommand();
    cmd.CommandText = sql;
    await using var reader = await cmd.ExecuteReaderAsync();
    while (await reader.ReadAsync())
    {
        // Assuming Edition has at least columns: id, Year
        var yearOrdinal = reader.GetOrdinal("Year");
        if (!reader.IsDBNull(yearOrdinal))
        {
            years.Add(reader.GetInt32(yearOrdinal));
        }
    }
    return years.OrderBy(y => y).ToList();
}

// Helper methods to query using Microsoft.Data.Sqlite without Dapper
static async Task<List<Track>> ReadTracksAsync(SqliteConnection conn, string sql)
{
    var tracks = new List<Track>();
    await using var cmd = conn.CreateCommand();
    cmd.CommandText = sql;
    await using var reader = await cmd.ExecuteReaderAsync();
    while (await reader.ReadAsync())
    {
        var track = new Track
        {
            Id = reader.GetInt32(reader.GetOrdinal("Id")),
            Title = reader.GetString(reader.GetOrdinal("Title")),
            Artist = reader.GetString(reader.GetOrdinal("Artist")),
            SearchTitle = reader.IsDBNull(reader.GetOrdinal("SearchTitle")) ? null : reader.GetString(reader.GetOrdinal("SearchTitle")),
            SearchArtist = reader.IsDBNull(reader.GetOrdinal("SearchArtist")) ? null : reader.GetString(reader.GetOrdinal("SearchArtist")),
            RecordedYear = reader.IsDBNull(reader.GetOrdinal("RecordedYear")) ? 0 : reader.GetInt32(reader.GetOrdinal("RecordedYear"))
        };
        tracks.Add(track);
    }
    return tracks;
}

static async Task<List<Listing>> ReadListingsAsync(SqliteConnection conn, string sql)
{
    var listings = new List<Listing>();
    await using var cmd = conn.CreateCommand();
    cmd.CommandText = sql;
    await using var reader = await cmd.ExecuteReaderAsync();
    while (await reader.ReadAsync())
    {
        var listing = new Listing
        {
            TrackId = reader.GetInt32(reader.GetOrdinal("TrackId")),
            Edition = reader.GetInt32(reader.GetOrdinal("Edition")),
            Position = reader.IsDBNull(reader.GetOrdinal("Position")) ? 0 : reader.GetInt32(reader.GetOrdinal("Position")),
            PlayUtcDateAndTime = reader.IsDBNull(reader.GetOrdinal("PlayUtcDateAndTime")) ? null : reader.GetDateTime(reader.GetOrdinal("PlayUtcDateAndTime"))
        };
        listings.Add(listing);
    }
    return listings;
}
