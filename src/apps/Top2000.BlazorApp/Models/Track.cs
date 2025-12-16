namespace Top2000.BlazorApp.Models;

public class Track
{
    public int Year { get; set; }
    public int Number { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Artist { get; set; } = string.Empty;
    public int TrackId { get; set; }
}
