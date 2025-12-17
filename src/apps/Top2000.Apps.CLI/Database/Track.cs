using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Top2000.Apps.CLI.Database;

public class Track
{
    [Key]
    public required int Id { get; init; }
    [MaxLength(100)]
    public required string Title { get; init; }
    [MaxLength(100)]
    public required string Artist { get; init; }
    public required int RecordedYear { get; init; }

    [MaxLength(100)]
    public string? SearchTitle { get; init; }
    
    [MaxLength(100)]
    public string? SearchArtist { get; init; }
    
    [JsonIgnore]
    public ICollection<Listing> Listings { get; init; } = [];
}