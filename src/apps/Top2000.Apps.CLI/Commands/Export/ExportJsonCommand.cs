using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Top2000.Apps.CLI.Database;
using Top2000.Data.JsonClientDatabase;
using Top2000.Data.JsonClientDatabase.Models;
using Top2000.Features;
using Top2000.Features.Listings;
using Edition = Top2000.Data.JsonClientDatabase.Models.Edition;
using Track = Top2000.Apps.CLI.Database.Track;

namespace Top2000.Apps.CLI.Commands.Export;


public class EditionExport
{
    [JsonPropertyName("y")]
    public required int Year { get; init; }
}

public class ListingExport
{
    [JsonPropertyName("t")]
    public required string Title { get; init; }
    [JsonPropertyName("a")]
    public required string Artist { get; init; }
    [JsonPropertyName("p")]
    public required int Position { get; init; }
    [JsonPropertyName("d")]
    public required int? Delta { get; init; }
    [JsonPropertyName("i")]
    public required string Icon { get; init; }
    [JsonPropertyName("c")]
    public required string IconColour { get; init; }
    
    [JsonPropertyName("g")]
    public required double? PlaygroupEpoch { get; init; }
    
    [JsonPropertyName("s")]
    public required string Slug { get; init; }

    public static string Transform(TrackListingDeltaType type)
    {
        return type switch
        {
            TrackListingDeltaType.NoChange => "equal",
            TrackListingDeltaType.Increased => "arrow_upward",
            TrackListingDeltaType.Decreased => "arrow_downward",
            TrackListingDeltaType.New => "flag",
            TrackListingDeltaType.Recurring => "replay",
            _ => "equal"
        };
    }

    public static string DeltaColour(TrackListingDeltaType type)
    {
        return type switch
        {
            TrackListingDeltaType.NoChange => "grey",
            TrackListingDeltaType.Increased =>  "green",
            TrackListingDeltaType.Decreased => "red",
            TrackListingDeltaType.New => "yellow",
            TrackListingDeltaType.Recurring => "yellow",
            _ => "grey"
        };
    }
}

public class ExportJsonCommand(Top2000DbContext dbContext, ITop2000Services top2000Services) : CommandBase("json", "Export data to Json format")
{
    protected override List<Symbol> Symbols =>
    [
        new Option<string>(name: "--output")
        {
            Description = "Output file path",
        }
    ];

    private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        WriteIndented = false,
        PropertyNamingPolicy = new ShortNameNamingPolicy(),
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

  
    private async Task<List<Edition>> GetAllEditionsAsync()
    {
        var allEditions = await top2000Services.AllEditionsAsync();

        return allEditions.Select(x => new Edition()
        {
            Year = x.Year,
            EndUtcDateAndTime = x.EndUtcDateAndTime,
            StartUtcDateAndTime = x.StartUtcDateAndTime,
            HasPlayDateAndTime = x.HasPlayDateAndTime
        }).ToList();
    }
    
    protected override async Task ExecuteAsync(ParseResult result, CancellationToken token)
    {
        var outputPath = result.GetValue<string>("--output") ?? "";

        // Single Progress session to avoid concurrency issues
        await AnsiConsole.Progress()
            .AutoRefresh(true)
            .HideCompleted(true)
            .Columns(new ProgressColumn[]
            {
                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new PercentageColumn(),
                new SpinnerColumn(Spinner.Known.Dots)
            })
            .StartAsync(async progressCtx =>
            {
                var taskExporting = progressCtx.AddTask("Exporting", autoStart: true);
                var allEditions = await GetAllEditionsAsync();
                var version = await top2000Services.DataVersion(token);

                var trackWithSlugs = new Dictionary<int, string>();
                var slugs = new HashSet<string>();
                var tracks = await dbContext.Tracks.ToListAsync(token);
                foreach (var track in tracks)
                {
                    var builder = new StringBuilder();

                    var title = string.IsNullOrWhiteSpace(track.SearchTitle)
                        ? track.Title
                        : track.SearchTitle;

                    var artist = string.IsNullOrWhiteSpace(track.SearchArtist)
                        ? track.Artist
                        : track.SearchArtist;

                    var slug = $"{title}-{artist}"
                        .Replace("!", "")
                        .Replace("@", "")
                        .Replace("#", "")
                        .Replace("$", "")
                        .Replace("%", "")
                        .Replace("^", "")
                        .Replace("&", "")
                        .Replace("*", "")
                        .Replace("(", "")
                        .Replace(")", "")
                        .Replace("_", "")
                        .Replace(".", "")
                        .Replace("=", "")
                        .Replace("+", "")
                        .Replace("{", "")
                        .Replace("}", "")
                        .Replace("|", "")
                        .Replace("/", "")
                        .Replace("\\", "")
                        .Replace("[", "")
                        .Replace("]", "")
                        .Replace("~", "")
                        .Replace("`", "")
                        .Replace("'", "")
                        .Replace("\"", "")
                        .Replace(":", "")
                        .Replace(";", "")
                        .Replace(" ", "-")
                        .ToLowerInvariant();

                    while (slug.Contains("--"))
                    {
                        slug = slug.Replace("--", "-");
                    }
                    
                    if (!slugs.Add(slug))
                    {
                        throw new Exception($"Slug {slug} already added");
                    }
                    
                    trackWithSlugs.Add(track.Id, slug);
                }
                
                var editions = allEditions.Select(x => new EditionExport
                    {
                        Year = x.Year
                    })
                    .ToList();

                await File.WriteAllTextAsync(Path.Combine(outputPath, "editions.json"), JsonSerializer.Serialize(editions), token);
                
                DateTime unixStart = new DateTime(1970, 1, 1);
                foreach (var edition in editions )
                {
                    var listings = await top2000Services.AllListingsOfEditionAsync(edition.Year, token);
                    var forExport = listings.Select(x => new ListingExport()
                        {
                            Artist = x.Artist,
                            Title = x.Title,
                            Position = x.Position,
                            Delta = x.Delta == 0 ? null : Math.Abs(x.Delta),
                            Icon = ListingExport.Transform(x.DeltaType),
                            IconColour = ListingExport.DeltaColour(x.DeltaType),
                            PlaygroupEpoch = (x.PlayUtcDateAndTime - unixStart).TotalSeconds,
                            Slug = trackWithSlugs[x.TrackId]
                        })
                        .ToList();

                    var json = JsonSerializer.Serialize(forExport);
                    await File.WriteAllTextAsync(Path.Combine(outputPath, edition.Year + ".json"), json, token);
                    
                }
                
            });
    }
}