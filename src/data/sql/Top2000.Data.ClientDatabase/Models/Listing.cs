namespace Top2000.Data.ClientDatabase.Models;

public class Listing
{
    public int TrackId { get; set; }
    public int Edition { get; set; }
    public int Position { get; set; }
    public DateTime? PlayUtcDateAndTime { get; set; }
}