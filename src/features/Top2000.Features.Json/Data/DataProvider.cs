using Top2000.Data.JsonClientDatabase.Models;

namespace Top2000.Features.Json.Data;

public class DataProvider
{
    public Top2000DataContext Value { get; } = new Top2000DataContext()
    {
        Version = 0,
        ListingsOfEdition = new Dictionary<int, List<Listing>>(),
        Listings = [],
        Tracks = new Dictionary<int, Track>(),
        Editions = []
    };
}