using System.Text;
using Top2000.Apps.CLI.Commands.Export.Isam;

namespace DownloaderApp;

public class Csv
{
    public static async Task MakeItAsync(List<EditionDbRecord> editions, List<TrackDbRecord> tracks, List<ListingDbRecord> poss)
    {
        var csvEditions = new List<string>()
        {
            "Year"
        };
        var csvTracks = new List<string>()
        {
            "Id,Title,Artist,Year,HighPosition,HighEdition,LowPosition,LowEdition,FirstPosition,FirstEdition,LastPosition,LastEdition,LastPlayTime,Appearances,AppearancesPositions"
        };
        var csvPossList = new List<string>
        {
            "Edition,Position,TrackId,Offset,OffsetType"
        };
        
        csvEditions.AddRange(editions.Select(e => e.ToCsvString()));
        csvTracks.AddRange(tracks.Select(t => t.ToCsvString()));
        csvPossList.AddRange(poss.Select(p => p.ToCsvString()));
        
        var utf8WithoutBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
        
        await File.WriteAllLinesAsync("editions.csv", csvEditions, utf8WithoutBom);
        await File.WriteAllLinesAsync("tracks.csv", csvTracks, utf8WithoutBom);
        await File.WriteAllLinesAsync("listings.csv", csvPossList, utf8WithoutBom);
    }
}