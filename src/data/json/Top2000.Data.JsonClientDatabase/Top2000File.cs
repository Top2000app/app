using Top2000.Data.JsonClientDatabase.Models;

namespace Top2000.Data.JsonClientDatabase;

public class Top2000File
{
    public List<Track> Tracks { get; set; } = [];
    public List<Listing> Listings { get; set; } = [];
    public List<Edition> Editions { get; set; } = [];
    public int Version { get; set; } = 0;
}