namespace Top2000.Apps.CLI.Database;

public class Listing
{
    public required int TrackId { get; init; }
    
    public required int EditionId { get; init; }
    public required int Position { get; init; }
    public DateTime? PlayUtcDateAndTime { get; init; }
    
    public required Track Track { get; init; }
    public required Edition Edition { get; init; }
}