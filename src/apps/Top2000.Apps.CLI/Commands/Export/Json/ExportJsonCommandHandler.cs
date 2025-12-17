using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Top2000.Apps.CLI.Database;
using Spectre.Console;

namespace Top2000.Apps.CLI.Commands.Export.Json;

public class ExportJsonCommandHandler
{
    private readonly Top2000DbContext _dbContext;
    private readonly JsonSerializerOptions _jsonOptions;

    public ExportJsonCommandHandler(Top2000DbContext dbContext)
    {
        _dbContext = dbContext;
        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = false,
            PropertyNamingPolicy = new ShortNameNamingPolicy(),
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    }

    public async Task<int> HandleExportJsonAsync(ParseResult result, CancellationToken token)
    {
        var outputPath = result.GetValue<string>("--output") ?? "json";
        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }

        List<Edition> editions = new();
        List<Listing> listings = new();
        List<Track> tracks = new();

        string? jsonString = null;
        string path = Path.Combine(outputPath, "top2000.json");

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
                    .ToListAsync(token);
                taskEditions.Value = 100;

                // Listings
                taskListings.StartTask();
                listings = await _dbContext.Listings
                    .AsNoTracking()
                    .ToListAsync(token);
                taskListings.Value = 100;

                // Tracks
                taskTracks.StartTask();
                tracks = await _dbContext.Tracks
                    .AsNoTracking()
                    .ToListAsync(token);
                taskTracks.Value = 100;

                // Serialize
                taskSerialize.StartTask();
                var dataContext = new Top2000DataContext
                {
                    Editions = editions,
                    Listings = listings,
                    Tracks = tracks
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

public class Top2000DataContext
{
    public required List<Track> Tracks { get; init; }
    public required List<Listing> Listings { get; init; }
    public required List<Edition> Editions { get; init; }
}
