namespace Top2000.Data.JsonClientDatabase.Models;

public class Listing
{
    public required int TrackId { get; init; }
    
    public required int EditionId { get; init; }
    public required int Position { get; init; }
    public DateTime? PlayUtcDateAndTime { get; init; }
    
    public required int Delta { get; init; }
    
    public required DeltaType DeltaType { get; init; }
    
}

public enum DeltaType
{
    NoChange  = 0,
    Increased = 1,
    Decreased = 2,
    New = 3,
    Recurring = 4
}