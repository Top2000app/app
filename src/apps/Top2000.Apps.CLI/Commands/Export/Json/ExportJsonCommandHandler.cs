using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Top2000.Apps.CLI.Database;
using Top2000.Data.JsonClientDatabase;
using Top2000.Features;


namespace Top2000.Apps.CLI.Commands.Export.Json;

public class ExportJsonCommandHandler
{
    private readonly Top2000DbContext _dbContext;
    private readonly Top2000Services _top2000Services;
    private readonly JsonSerializerOptions _jsonOptions;

    public ExportJsonCommandHandler(Top2000DbContext dbContext, Top2000Services top2000Services)
    {
        _dbContext = dbContext;
        _top2000Services = top2000Services;
        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = false,
            PropertyNamingPolicy = new ShortNameNamingPolicy(),
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    }

    public async Task<int> HandleExportJsonAsync(ParseResult result, CancellationToken token)
    {
        var outputPath = result.GetValue<string>("--output") ?? "";

        List<Top2000.Data.JsonClientDatabase.Models.Edition> editions = [];
        List<Top2000.Data.JsonClientDatabase.Models.Listing> listings = [];
        List<Top2000.Data.JsonClientDatabase.Models.Track> tracks = [];

        string? jsonString = null;
        var path = Path.Combine(outputPath, "top2000.json");

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
                var taskEditions = progressCtx.AddTask("Loading editions", autoStart: true);
                var taskListings = progressCtx.AddTask("Loading listings", autoStart: false);
                var taskTracks = progressCtx.AddTask("Loading tracks", autoStart: false);
                var taskSerialize = progressCtx.AddTask("Serializing JSON", autoStart: false);
                var taskWrite = progressCtx.AddTask("Writing file", autoStart: false);

                // Editions
                editions = await _dbContext.Editions
                    .AsNoTracking()
                    .Select(x => new Top2000.Data.JsonClientDatabase.Models.Edition()
                    {
                        Year = x.Year,
                        EndUtcDateAndTime = x.EndUtcDateAndTime,
                        HasPlayDateAndTime =x.HasPlayDateAndTime,
                        StartUtcDateAndTime = x.StartUtcDateAndTime
                    })
                    .ToListAsync(token);
                taskEditions.Value = 100;

                // Listings
                taskListings.StartTask();
                listings = await _dbContext.Listings
                    .AsNoTracking()
                    .Select(x => new Top2000.Data.JsonClientDatabase.Models.Listing
                    {
                        EditionId = x.EditionId,
                        Position = x.Position,
                        TrackId = x.TrackId,
                        PlayUtcDateAndTime = x.PlayUtcDateAndTime
                    })
                    .ToListAsync(token);
                taskListings.Value = 100;

                // Tracks
                taskTracks.StartTask();
                tracks = await _dbContext.Tracks
                    .AsNoTracking()
                    .Select(x => new Top2000.Data.JsonClientDatabase.Models.Track()
                    {
                        Artist =  x.Artist,
                        Id = x.Id,
                        RecordedYear = x.RecordedYear,
                        Title = x.Title,
                        SearchArtist =  x.SearchArtist,
                        SearchTitle = x.SearchTitle,
                    })
                    .ToListAsync(token);
                taskTracks.Value = 100;

                // Serialize
                taskSerialize.StartTask();
                var dataContext = new Data.JsonClientDatabase.Models.Top2000DataContext
                {
                    Editions = editions,
                    Listings = listings,
                    Tracks = tracks,
                    Version = await _top2000Services.DataVersion(token)
                };
                jsonString = JsonSerializer.Serialize(dataContext, _jsonOptions);
                taskSerialize.Value = 100;

                // Write
                taskWrite.StartTask();
                await File.WriteAllTextAsync(path, jsonString, token);
                taskWrite.Value = 100;
            });

        // Summary panel
        var table = new Table().Border(TableBorder.Rounded).BorderColor(Color.Green);
        table.AddColumn(new TableColumn("Dataset").Centered());
        table.AddColumn(new TableColumn("Count").Centered());
        table.AddRow("Editions", editions.Count.ToString());
        table.AddRow("Listings", listings.Count.ToString());
        table.AddRow("Tracks", tracks.Count.ToString());

        var panel = new Panel(table)
        {
            Header = new PanelHeader("Export complete"),
            Border = BoxBorder.Rounded,
            Padding = new Padding(1, 1, 1, 1)
        };

        AnsiConsole.Write(panel);
        AnsiConsole.MarkupLine($"[grey]Output:[/] [bold]{path}[/]");

        return 0;
    }
}
