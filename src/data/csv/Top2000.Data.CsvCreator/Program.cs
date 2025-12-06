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
var connection = services.GetRequiredService<SQLiteAsyncConnection>();

var allTrackSql = "SELECT * FROM Track";
var allEditionsSql = "SELECT * FROM Edition";
var allListingSql = "SELECT * FROM Listing";

var allTracks = await connection.QueryAsync<Track>(allTrackSql);
var allListings = await connection.QueryAsync<Listing>(allListingSql);
var allEditions = (await connection.QueryAsync<Edition>(allEditionsSql))
    .OrderBy(x => x.Year)
    .Select(x => x.Year)
    .ToList();

var lastYear = allEditions[allEditions.Count - 1];

var lines = new List<string>
{
    $"Id;Title;Artist;SearchTitle;SearchArtist;RecordedYear;LastPlayTimeUtc;{string.Join(';', allEditions)}"
};

var line = new StringBuilder();

foreach (var track in allTracks)
{
    var trackListings = allListings
        .Where(x => x.TrackId == track.Id)
        .ToList();

    line = new StringBuilder($"{track.Id};{track.Title};{track.Artist};{track.SearchTitle ?? ""};{track.SearchArtist ?? ""};{track.RecordedYear};");

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

await File.WriteAllLinesAsync("top2000.csv", lines, encoding: System.Text.Encoding.UTF8);
