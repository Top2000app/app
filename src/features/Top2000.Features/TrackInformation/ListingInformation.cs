namespace Top2000.Features.TrackInformation;

public class ListingInformation
{
    public int Edition { get; set; }

    public int? Position { get; set; }

    public DateTime? PlayUtcDateAndTime { get; set; }

    public DateTime? LocalUtcDateAndTime => PlayUtcDateAndTime is DateTime utcDateTime
        ? DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc).ToLocalTime()
        : null;

    public int? Offset { get; set; }

    public ListingStatus Status { get; set; }

    public bool CouldBeListed(int recoredYear) => recoredYear <= Edition;
}
