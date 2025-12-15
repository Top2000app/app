using System.ComponentModel.Design.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Top2000.Apps.CLI.Database;
using Top2000.Features.AllListingsOfEdition;
using Top2000.Features.TrackInformation;

namespace Top2000.Apps.CLI.Commands.Export.json;

public class ExportJsonCommandHandler
{
    private readonly Top2000DbContext _dbContext;
    private readonly IMediator _mediator;
    private readonly JsonSerializerOptions _jsonOptions;

    public ExportJsonCommandHandler(Top2000DbContext dbContext, IMediator mediator)
    {
        _dbContext = dbContext;
        _mediator = mediator;
        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    }
    
    public async Task<int> HandleExportJsonAsync(ParseResult result, CancellationToken token)
    {
        var outputPath = result.GetValue<string>("--output") ?? "json";
        // Ensure output directory exists
        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }
        
        
        AnsiConsole.Write(new Rule("[green]Top2000 JSON Export[/]").RuleStyle("grey").LeftJustified());
        AnsiConsole.WriteLine();

        // Load all editions first
        var editions = await _dbContext.Editions
            .OrderBy(e => e.Year)
            .Select(e => e.Year)
            .ToListAsync(token);

        var trackIds = await _dbContext.Tracks
            .Select(x => x.Id)
            .ToListAsync(token);

        AnsiConsole.MarkupLine($"[blue]Found {editions.Count} editions to export: {editions.First()}-{editions.Last()}[/]");
        AnsiConsole.WriteLine();

        var exportedFiles = new List<string>();
        var totalTracks = 0;

        await AnsiConsole.Progress()
            .StartAsync(async ctx =>
            {
                var mainTask = ctx.AddTask("[green]Exporting editions to JSON[/]", maxValue: editions.Count);
                
                foreach (var edition in editions)
                {
                    var editionTask = ctx.AddTask($"[blue]Processing {edition}...[/]");
                    editionTask.StartTask();
                    
                    // Get listings for this edition
                    var allListings = await _mediator.Send(new AllListingsOfEditionRequest
                    {
                        Year = edition
                    }, token);

                    // Convert HashSet to List for better JSON serialization and ordering
                    var sortedListings = allListings
                        .OrderBy(x => x.Position)
                        .ToList();
                    
                    editionTask.Description = $"[blue]Serializing {edition} ({sortedListings.Count:N0} tracks)...[/]";

                    // Serialize to JSON
                    var jsonString = JsonSerializer.Serialize(sortedListings, _jsonOptions);
                    
                    // Determine output path
                    var fileName = $"{edition}.json";
                    var filePath = Path.Combine(outputPath, fileName);

                    // Write JSON file
                    await File.WriteAllTextAsync(filePath, jsonString, token);
                    
                    exportedFiles.Add(Path.GetFullPath(filePath));
                    totalTracks += sortedListings.Count;
                    
                    editionTask.Value = 100;
                    mainTask.Increment(1);
                    
                    AnsiConsole.MarkupLine($"✓ [green]{edition}.json[/] - [cyan]{sortedListings.Count:N0} tracks[/] - [yellow]{GetFileSize(filePath)}[/]");
                }
                
                mainTask.Value = editions.Count;
            });

        
        var versionInfo = new
        {
            LatestEdition = editions.Last(),
            Editions = editions.ToList(),
            Version = 63
        };

        var versionFilePath = Path.Combine(outputPath, "version.json");
        var versionJsonString = JsonSerializer.Serialize(versionInfo, _jsonOptions);
        await File.WriteAllTextAsync(versionFilePath, versionJsonString, token);
        
        exportedFiles.Add(Path.GetFullPath(versionFilePath));
        AnsiConsole.MarkupLine($"✓ [green]version.json[/] - [cyan]metadata[/] - [yellow]{GetFileSize(versionFilePath)}[/]");
        
        // Success summary
        AnsiConsole.WriteLine();
        var summaryContent = $"[green]✓ JSON export completed successfully![/]\n\n" +
                           $"[bold]Files exported:[/] [cyan]{exportedFiles.Count}[/] ([cyan]{exportedFiles.Count - 1}[/] editions + [cyan]1[/] version file)\n" +
                           $"[bold]Total tracks:[/] [cyan]{totalTracks:N0}[/]\n" +
                           $"[bold]Output directory:[/] [yellow]{Path.GetFullPath(outputPath)}[/]\n\n" +
                           $"[bold]Exported files:[/]\n" + 
                           string.Join("\n", exportedFiles.Select(f => $"  • [yellow]{Path.GetFileName(f)}[/]"));

        var panel = new Panel(summaryContent)
            .Header("[green]Export Complete[/]")
            .BorderColor(Color.Green);
        
        AnsiConsole.Write(panel);

        return 1;
    }

    private static string GetFileSize(string filePath)
    {
        var fileInfo = new FileInfo(filePath);
        var bytes = fileInfo.Length;
        
        string[] sizes = ["B", "KB", "MB", "GB"];
        int order = 0;
        double size = bytes;
        
        while (size >= 1024 && order < sizes.Length - 1)
        {
            order++;
            size /= 1024;
        }
        
        return $"{size:0.##} {sizes[order]}";
    }
}