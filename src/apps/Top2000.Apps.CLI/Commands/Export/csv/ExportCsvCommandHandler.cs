using System.Text;
using Microsoft.EntityFrameworkCore;
using Top2000.Apps.CLI.Database;

namespace Top2000.Apps.CLI.Commands.Export.csv;

public class ExportCsvCommandHandler
{
    private readonly Top2000DbContext _dbContext;

    public ExportCsvCommandHandler(Top2000DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> HandleExportCsvAsync(ParseResult result, CancellationToken token)
    {
        var outputPath = result.GetValue<string>("--output");
        
        AnsiConsole.Write(new Rule("[green]Top2000 CSV Export[/]").RuleStyle("grey").LeftJustified());
        AnsiConsole.WriteLine();

        await AnsiConsole.Progress()
            .StartAsync(async ctx =>
            {
                var mainTask = ctx.AddTask("[green]Exporting Top2000 data to CSV[/]");
                
                // Step 1: Load tracks
                var trackTask = ctx.AddTask("[blue]Loading tracks from database...[/]");
                trackTask.StartTask();
                
                var allTracks = await _dbContext.Tracks
                    .Include(t => t.Listings)
                    .ThenInclude(l => l.Edition)
                    .ToListAsync(token);
                
                trackTask.Value = 100;
                AnsiConsole.MarkupLine($"✓ [green]Loaded {allTracks.Count:N0} tracks[/]");
                
                // Step 2: Load editions
                var editionTask = ctx.AddTask("[blue]Loading editions from database...[/]");
                editionTask.StartTask();
                
                var allEditions = await _dbContext.Editions
                    .OrderBy(e => e.Year)
                    .Select(e => e.Year)
                    .ToListAsync(token);
                
                editionTask.Value = 100;
                var lastYear = allEditions.Last();
                AnsiConsole.MarkupLine($"✓ [green]Loaded {allEditions.Count} editions ({allEditions.First()}-{lastYear})[/]");
                
                // Step 3: Generate CSV lines
                var csvTask = ctx.AddTask("[blue]Generating CSV data...[/]", maxValue: allTracks.Count);
                
                var lines = new List<string>();
                var batchSize = Math.Max(1, allTracks.Count / 100); // Update progress every 1%
                
                for (int i = 0; i < allTracks.Count; i++)
                {
                    lines.Add(CreateCsvString(allTracks[i], allEditions, lastYear));
                    
                    if (i % batchSize == 0 || i == allTracks.Count - 1)
                    {
                        csvTask.Value = i + 1;
                    }
                }
                
                // Add header
                lines.Insert(0, $"Id;Title;Artist;SearchTitle;SearchArtist;RecordedYear;LastPlayTimeUtc;{string.Join(';', allEditions)}");
                AnsiConsole.MarkupLine($"✓ [green]Generated {lines.Count - 1:N0} data rows[/]");
                
                // Step 4: Write file
                var writeTask = ctx.AddTask("[blue]Writing CSV file...[/]");
                writeTask.StartTask();
                
                var path = "top2000.csv";
                if (!string.IsNullOrWhiteSpace(outputPath))
                {
                    path = Path.Combine(outputPath, path);
                }
                
                await File.WriteAllLinesAsync(path, lines, encoding: Encoding.UTF8, cancellationToken: token);
                writeTask.Value = 100;
                
                mainTask.Value = 100;
                
                // Success message
                AnsiConsole.WriteLine();
                var panel = new Panel($"[green]✓ CSV export completed successfully![/]\n\n[bold]File location:[/] [yellow]{Path.GetFullPath(path)}[/]\n[bold]Total records:[/] [cyan]{allTracks.Count:N0}[/]\n[bold]File size:[/] [cyan]{GetFileSize(path)}[/]")
                    .Header("[green]Export Complete[/]")
                    .BorderColor(Color.Green);
                
                AnsiConsole.Write(panel);
            });

        return 1;
    }

    private static string CreateCsvString(Track track, List<int> allEditions, int lastYear)
    {
        var line = new StringBuilder($"{track.Id};{track.Title};{track.Artist};{track.SearchTitle ?? ""};{track.SearchArtist ?? ""};{track.RecordedYear};");

        var lastPlayUtcDateAndTime = track.Listings.FirstOrDefault(x => x.EditionId == lastYear)?.PlayUtcDateAndTime;
        line.Append(lastPlayUtcDateAndTime?.ToString("dd-MM-yyyy HH:mm:ss'Z'") ?? string.Empty);

        var allPositions = allEditions
            .Select(year => track.Listings.FirstOrDefault(x => x.EditionId == year))
            .Select(listing => listing?.Position.ToString() ?? "");
            
        foreach (var position in allPositions)
        {
            line.Append($";{position}");
        }

        return line.ToString();
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