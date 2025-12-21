namespace Top2000.Data.JsonClientDatabase.Models;

public class Listing
{
    public required int TrackId { get; init; }
    
    public required int EditionId { get; init; }
    public required int Position { get; init; }
    public DateTime? PlayUtcDateAndTime { get; init; }
}