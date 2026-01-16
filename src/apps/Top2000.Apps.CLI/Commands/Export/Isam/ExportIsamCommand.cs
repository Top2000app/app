using System.Text;
using Microsoft.EntityFrameworkCore;
using Top2000.Apps.CLI.Database;
using Top2000.Features;
using Top2000.Features.TrackInformation;
using TrackDetails = DownloaderApp.TrackDetails;

namespace Top2000.Apps.CLI.Commands.Export.Isam;

public class ExportIsamCommand(Top2000Services _top2000Services, Top2000DbContext top2000DbContext) : CommandBase("isam", "Export the DOS ISAM database for the Top2000")
{
    protected override List<Symbol> Symbols =>
    [
        new Option<string>("--output")
        {
            Description = "Output file directory"
        }
    ];

    protected override async Task ExecuteAsync(ParseResult result, CancellationToken token)
    {
        var trackIds = await AllTrackIdsAsync();
        
        var listings = new List<string>();
        var tracks = new List<string>();
        var editions = (await _top2000Services.AllEditionsAsync(CancellationToken.None))
            .Select(x => $"{x.Year}")
            .ToList();
        
        foreach (var trackId in trackIds)
        {
            var details = await TrackDetailsAsync(trackId);
            tracks.Add(TrackDbRecord.ToTrackDbRecord(trackId, details).ToCsvString());
            
            var listingsForTrack = details.Listings
                .Where(x => x.Position is > 0)
                .OrderBy(x => x.Edition)
                .Select(x => new ListingDbRecord
                {
                    Edition = x.Edition,
                    Position = x.Position!.Value,
                    TrackId = trackId,
                    Offset = ListingDbRecord.ReadOffSet(x.Offset),
                    OffsetType = ListingDbRecord.ToChr(x.Status)
                }.ToCsvString())
                .ToList();
            
            listings.AddRange(listingsForTrack);
        }
        
        editions.Insert(0, "Year");
        tracks.Insert(0, "Id,Title,Artist,Year,HighPosition,HighEdition,LowPosition,LowEdition,FirstPosition,FirstEdition,LastPosition,LastEdition,LastPlayTime,Appearances,AppearancesPositions");
        listings.Insert(0, "Edition,Position,TrackId,Offset,OffsetType");
        
        var utf8WithoutBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
        
        await File.WriteAllLinesAsync("editions.csv", editions, utf8WithoutBom, token);
        await File.WriteAllLinesAsync("tracks.csv", tracks, utf8WithoutBom, token);
        await File.WriteAllLinesAsync("listings.csv", listings, utf8WithoutBom, token);
    }
    
    private Task<List<int>> AllTrackIdsAsync()
    {
        return top2000DbContext.Tracks
            .OrderBy(x => x.Id)
            .Select(x => x.Id)
            .ToListAsync();
    }
    
    private async Task<TrackDetails> TrackDetailsAsync(int trackId)
    {
        var trackDetails = await _top2000Services.TrackDetailsAsync(trackId);

        return new TrackDetails
        {
            Title = trackDetails.Title,
            Artist = trackDetails.Artist,
            RecordedYear = trackDetails.RecordedYear,
            Listings = new SortedSet<ListingInformation>(trackDetails.Listings)
        };
    }
}