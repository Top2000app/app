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

        var allTracks = await _dbContext.Tracks
            .Include(t => t.Listings)
            .ThenInclude(l => l.Edition)
            .ToListAsync(token);

        var allEditions = await _dbContext.Editions
            .OrderBy(e => e.Year)
            .Select(e => e.Year)
            .ToListAsync(token);

        var lastYear = allEditions.Last();

        var lines = allTracks
            .Select(x => CreateCsvString(x, allEditions, lastYear))
            .ToList();
            
        lines.Insert(0, $"Id;Title;Artist;SearchTitle;SearchArtist;RecordedYear;LastPlayTimeUtc;{string.Join(';', allEditions)}");
        
        var path = "top2000.csv";
        
        if (!string.IsNullOrWhiteSpace(outputPath))
        {
            path = Path.Combine(outputPath, path);
        }
        
        await File.WriteAllLinesAsync(path, lines, encoding: Encoding.UTF8, cancellationToken: token);

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
       
}