namespace Top2000.Data.JsonClientDatabase.Models;

public class Top2000DataContext
{
    public required int Version { get; init; } 

    public required Dictionary<int, List<Listing>> ListingsOfEdition { get; init;  } 
    public required List<Listing> Listings { get; init;  } 
    
    public required Dictionary<int, Track> Tracks { get; set; } 
    
    public required List<Edition> Editions { get; set; }
}

