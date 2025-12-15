using System.ComponentModel.DataAnnotations;

namespace Top2000.Apps.CLI.Database;

public class Edition
{
    [Key]
    public required int Year { get; init; }
    public required DateTime StartUtcDateAndTime { get; init; }
    public required DateTime EndUtcDateAndTime { get; init; }
    public required bool HasPlayDateAndTime { get; init; }
    
    public ICollection<Listing> Listings { get; init; } = [];
}