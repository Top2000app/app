namespace Top2000.Features.AllListingsOfEdition;

public class TrackListing
{
    public required int TrackId { get; init; }

    public required int Position { get; init; }

    public int? Delta { get; init; }

    public required bool IsRecurring { get; init; }

    public DateTime PlayUtcDateAndTime { get; init; }

    public required string Title { get; init; }

    public required string Artist { get; init; }
}
