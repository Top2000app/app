using DownloaderApp;

public class ListingInformation
{
    public int Edition { get; set; }

    public int? Position { get; set; }

    public DateTime? PlayUtcDateAndTime { get; set; }

    public DateTime? LocalUtcDateAndTime
    {
        get
        {
            if (PlayUtcDateAndTime is null) return null;
            var utcTime = DateTime.SpecifyKind((DateTime)PlayUtcDateAndTime, DateTimeKind.Utc);

            // we can make this shortcut since I only show the CET in the MS-DOS app.
            return utcTime.AddHours(1);
        }
    }

    public int? Offset { get; set; }

    public ListingStatus Status { get; set; }

    public bool CouldBeListed(int recoredYear) => recoredYear <= Edition;
}
