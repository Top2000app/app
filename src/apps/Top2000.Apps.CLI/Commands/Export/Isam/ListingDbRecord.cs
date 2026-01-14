using DownloaderApp;

namespace Top2000.Apps.CLI.Commands.Export.Isam;

public class ListingDbRecord
{
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

    public int Edition { get; set; }
    public int Offset { get; set; }
    public int Position { get; set; }
    public int TrackId { get; set; }
    public int OffsetType { get; set; }
}