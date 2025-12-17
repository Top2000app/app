using System.Text.Json.Serialization;

namespace Top2000.Apps.CLI.Database;

public class Listing
{
    public required int TrackId { get; init; }
    
    public required int EditionId { get; init; }
    public required int Position { get; init; }
    public DateTime? PlayUtcDateAndTime { get; init; }
    
    [JsonIgnore]
    public Track? Track { get; init; }
    
    [JsonIgnore]
    public Edition? Edition { get; init; }
}