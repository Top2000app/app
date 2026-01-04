using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Top2000.Apps.CLI.Database;
using Top2000.Data.JsonClientDatabase;
using Top2000.Data.JsonClientDatabase.Models;
using Top2000.Features;
using Edition = Top2000.Data.JsonClientDatabase.Models.Edition;

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

    private async Task<List<Edition>> GetAllEditionsAsync()
    {
        var allEditions = await _top2000Services.AllEditionsAsync();

        return allEditions.Select(x => new Edition()
        {
            Year = x.Year,
            EndUtcDateAndTime = x.EndUtcDateAndTime,
            StartUtcDateAndTime = x.StartUtcDateAndTime,
            HasPlayDateAndTime = x.HasPlayDateAndTime
        }).ToList();
    }
    
     private async Task<int> HandleExportJsonAsync(ParseResult result, CancellationToken token)
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
                var version = await _top2000Services.DataVersion(token);

                var fileInfo = new Top2000VersionInfo
                {
                    Editions = allEditions,
                    Version = version
                };

                var tracks = await _dbContext.Tracks
                    .AsNoTracking()
                    .Select(x => new Top2000.Data.JsonClientDatabase.Models.Track()
                    {
                        Artist = x.Artist,
                        Id = x.Id,
                        RecordedYear = x.RecordedYear,
                        Title = x.Title,
                        SearchArtist = x.SearchArtist,
                        SearchTitle = x.SearchTitle,
                    })
                    .ToDictionaryAsync(x => x.Id, token);

                foreach (var edition in allEditions)
                {
                    var listingOfEdition = (await _top2000Services.AllListingsOfEditionAsync(edition.Year, token))
                        .Select(listing => new Data.JsonClientDatabase.Models.Listing
                        {
                            EditionId = edition.Year,
                            Position = listing.Position,
                            TrackId = listing.TrackId,
                            PlayUtcDateAndTime = listing.PlayUtcDateAndTime,
                            Delta = listing.Delta,
                            DeltaType = (DeltaType)listing.DeltaType,
                            RecordedYear = tracks[listing.TrackId].RecordedYear,
                            SearchArtist = tracks[listing.TrackId].SearchArtist,
                            SearchTitle = tracks[listing.TrackId].SearchTitle,
                            Artist = tracks[listing.TrackId].Artist,
                            Title = tracks[listing.TrackId].Title,
                        })
                        .ToList();

                    var fileContent = new Top2000File
                    {
                        Listings = listingOfEdition,
                    };

                    var jsonString = JsonSerializer.Serialize(fileContent, _jsonOptions);
                    var path = Path.Combine(outputPath, $"{edition.Year}.json");
                    await File.WriteAllTextAsync(path, jsonString, token);
                }
                
                var versionPath = Path.Combine(outputPath, "version.json");
                var jsonVersionString = JsonSerializer.Serialize(fileInfo, _jsonOptions);
                await File.WriteAllTextAsync(versionPath, jsonVersionString, token);
                taskExporting.Value = 100;
            });


        return 0;
    }
}