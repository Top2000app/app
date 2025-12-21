namespace Top2000.Data.JsonClientDatabase.Models;

public class Top2000DataContext
{
    public List<Track> Tracks { get; set; } = [];
    public List<Listing> Listings { get; set; } = [];
    public List<Edition> Editions { get; set; } = [];

    public int Version { get; set; } = 0;
}

