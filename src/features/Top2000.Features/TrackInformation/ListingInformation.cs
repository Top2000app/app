namespace Top2000.Features.TrackInformation;

public class ListingInformation
{
    public required int Edition { get; init; }

    public int? Position { get; init; }

    public DateTime? PlayUtcDateAndTime { get; init; }

    public DateTime? LocalUtcDateAndTime => PlayUtcDateAndTime is DateTime utcDateTime
        ? DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc).ToLocalTime()
        : null;

    public int? Offset { get; set; }

    public ListingStatus Status { get; set; }

    public bool CouldBeListed(int recoredYear) => recoredYear <= Edition;
}
