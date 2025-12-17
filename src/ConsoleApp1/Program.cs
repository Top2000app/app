using System.Runtime.CompilerServices;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Top2000.Apps.CLI.Database;
using Top2000.Data.ClientDatabase;
using Top2000.Features;
using Top2000.Features.SQLite;

var adapter = new SqliteFeatureAdapter();
var configurationManager = new ConfigurationManager();

var services = new ServiceCollection()
    .AddDbContext<Top2000DbContext>()
    .AddTop2000Features(configurationManager, adapter)
    .BuildServiceProvider();

var databaseGen = services.GetRequiredService<IUpdateClientDatabase>();
var top2000 = services.GetRequiredService<Top2000AssemblyDataSource>();
await databaseGen.RunAsync(top2000);

var db = services.GetRequiredService<Top2000DbContext>();

// Read JSON file content
var jsonContent = await File.ReadAllTextAsync("2025.json");

// Deserialize JSON into RootObject
var rootObject = JsonSerializer.Deserialize<RootObject>(jsonContent)
    ?? throw new InvalidOperationException("Doesn't deserialize");

var positions = rootObject.positions
    .OrderBy(x => x.position.current)
    .ToList();

var lastEdition = db.Listings
    .Where(x => x.EditionId == 2024)
    .ToList();

var tracks = db.Tracks.ToList();
var lines = new List<string>();
var counter = 0;
foreach (var item in positions)
{
    if (counter == 0)
    {
        lines.Add("INSERT INTO [Listing] ([TrackId] ,[Edition] ,[Position], [PlayUtcDateAndTime]) VALUES");
        counter++;
    }
    
    var terminator = counter == 500 ? ";" : ",";
    
    var formattedTime = item.broadcastTimeFormatted.Split(' ');
    var hour  = int.Parse(formattedTime[4].Split(':')[0]);
    var day = int.Parse(formattedTime[1]);
    var dateTime = new DateTime(2025, 12, day, hour, 0, 0, DateTimeKind.Local);
    var utcTime = dateTime.ToUniversalTime();
    var time = $"2025-12-{utcTime.Day}T{utcTime.Hour.ToString().PadLeft(2,'0')}:00:00";
    
    if (item.position.previous != 0)
    {
        item.track.myId = lastEdition.Single(x => x.Position == item.position.previous).TrackId;
        lines.Add($"({item.track.myId},2025,{item.position.current},'{time}'){terminator}");
    }
    else
    {
        item.track.myId = tracks
            .SingleOrDefault(x => x.Artist == item.track.artist && x.Title == item.track.title)
            ?.Id ?? 0;

        if (item.track.myId == 0)
        {
            lines.Add($"({item.track.title}-{item.track.artist},2025,{item.position.current},'{time}'){terminator}");
        }
        else
        {
            lines.Add($"({item.track.myId},2025,{item.position.current},'{time}'){terminator}");
        }
        
    }

    counter++;
    if (counter > 500)
    {
        counter = 0;
        lines.Add("");
    }
    //
    // Console.WriteLine($"{item.position.current};{item.track.title};{item.track.artist}");
}

await File.WriteAllLinesAsync("0065-EditionOf2025.sql", lines);