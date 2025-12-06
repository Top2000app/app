using Top2000.Data.Csv.Models;

namespace Top2000.Data.Csv;

public class Top2000DataContext
{
    public List<Track> Tracks { get; set; } = [];
    public List<Listing> Listings { get; set;  } = [];
    public List<Edition> Editions { get; set;  } = [];
}
