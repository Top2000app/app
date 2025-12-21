namespace Top2000.Data.JsonClientDatabase.Models;

public class Track
{
    public required int Id { get; init; }
    public required string Title { get; init; }
    public required string Artist { get; init; }
    public required int RecordedYear { get; init; }

    public string? SearchTitle { get; init; }
    
    public string? SearchArtist { get; init; }
}