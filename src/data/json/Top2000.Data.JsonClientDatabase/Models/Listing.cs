using System.Xml;

namespace Top2000.Data.JsonClientDatabase.Models;

public class Listing
{
    public required int TrackId { get; init; }
    
    public required int EditionId { get; init; }
    public required int Position { get; init; }
    public DateTime? PlayUtcDateAndTime { get; init; }
    
    public required int Delta { get; init; }
    
    public required DeltaType DeltaType { get; init; }
    
    public required int RecordedYear { get; init; }
    
    public required string Title { get; init; }
    public required string Artist { get; init; }

    public string? SearchTitle { get; init; }
    
    public string? SearchArtist { get; init; }
}

public enum DeltaType
{
    NoChange  = 0,
    Increased = 1,
    Decreased = 2,
    New = 3,
    Recurring = 4
}