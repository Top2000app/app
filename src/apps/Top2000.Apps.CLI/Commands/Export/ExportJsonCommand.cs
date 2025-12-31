using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Top2000.Apps.CLI.Database;
using Top2000.Data.JsonClientDatabase;
using Top2000.Data.JsonClientDatabase.Models;
using Top2000.Features;

namespace Top2000.Apps.CLI.Commands.Export;

public class ExportJsonCommand : ICommand<ExportCommands>
{
    private readonly Top2000DbContext _dbContext;
    private readonly Top2000Services _top2000Services;
    private readonly JsonSerializerOptions _jsonOptions;

    public ExportJsonCommand(Top2000DbContext dbContext, Top2000Services top2000Services)
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
    
    public Command Create()
    {
        var csvCommand = new Command("json", "Export to Json format");
        var outputOption = new Option<string>(name: "--output", "-o", "/o")
        {
            Description = "Output file path",
        };
        csvCommand.Add(outputOption);
        
        csvCommand.SetAction(HandleExportJsonAsync);

        return csvCommand;
    }
    
     private async Task<int> HandleExportJsonAsync(ParseResult result, CancellationToken token)
    {
        var outputPath = result.GetValue<string>("--output") ?? "";

        var fileContent = new Top2000File();

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
                var taskExporting = progressCtx.AddTask("Exporting", autoStart: true);
                var allEditions = await _top2000Services.AllEditionsAsync(token);

                foreach (var edition in allEditions)
                {
                    fileContent.Editions.Add(new Data.JsonClientDatabase.Models.Edition()
                    {
                        Year = edition.Year,
                        EndUtcDateAndTime = edition.EndUtcDateAndTime,
                        StartUtcDateAndTime = edition.StartUtcDateAndTime,
                        HasPlayDateAndTime = edition.HasPlayDateAndTime
                    });

                    var listingOfEdition = (await _top2000Services.AllListingsOfEditionAsync(edition.Year, token))
                        .Select(listing => new Top2000.Data.JsonClientDatabase.Models.Listing
                        {
                            EditionId = edition.Year,
                            Position = listing.Position,
                            TrackId = listing.TrackId,
                            PlayUtcDateAndTime = listing.PlayUtcDateAndTime,
                            Delta = listing.Delta,
                            DeltaType = (DeltaType)listing.DeltaType
                        })
                        .ToList();
                    fileContent.Listings.AddRange(listingOfEdition);
                }
                

                // Tracks
                var tracks = await _dbContext.Tracks
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
                fileContent.Tracks.AddRange(tracks);

                // Serialize

                fileContent.Version= await _top2000Services.DataVersion(token);
                jsonString = JsonSerializer.Serialize(fileContent, _jsonOptions);
                // Write
                await File.WriteAllTextAsync(path, jsonString, token);
                taskExporting.Value = 100;
            });

        AnsiConsole.MarkupLine($"[grey]Output:[/] [bold]{path}[/]");

        return 0;
    }
}