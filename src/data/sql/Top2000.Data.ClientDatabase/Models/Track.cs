namespace Top2000.Data.ClientDatabase.Models;

public class Track
{
    [Key]
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Artist { get; set; } = string.Empty;

    public string? SearchTitle { get; set; }
    public string? SearchArtist { get; set; }
    public int RecordedYear { get; set; }
}
