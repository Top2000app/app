using DownloaderApp;

namespace Top2000.Apps.CLI.Commands.Export.Isam;

public class ListingDbRecord
{
    public required int Edition { get; init; }
    public required int Offset { get; init; }
    public required int Position { get; init; }
    public required int TrackId { get; init; }
    public required int OffsetType { get; init; }
    
    public static int ReadOffSet(int? value)
    {
        return value.HasValue
            ? Math.Abs(value.Value)
            : 0;
    }

    public static int ToChr(ListingStatus status)
    {
        return status switch
        {
            ListingStatus.New => 14,
            ListingStatus.Decreased => 31,
            ListingStatus.Increased => 30,
            ListingStatus.Unchanged => 61,
            ListingStatus.Back => 27,
            _ => throw new Exception(),
        };
    }

    public string ToCsvString()
    {
        return $"{Edition},{Position},{TrackId},{Offset},{OffsetType}";
    }
}