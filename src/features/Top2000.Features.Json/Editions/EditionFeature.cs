using Top2000.Data.JsonClientDatabase.Models;
using Top2000.Features.Editions;
using Edition = Top2000.Features.Editions.Edition;

namespace Top2000.Features.Json.Editions;

public class EditionFeature : IEditions
{
    private readonly Top2000DataContext _data;

    public EditionFeature(Top2000DataContext data)
    {
        _data = data;
    }
    
    public Task<SortedSet<Edition>> AllEditionsAsync(CancellationToken cancellationToken = default)
    {
        var resultSet = _data.Editions
            .Select(x => new Edition
            {
                Year = x.Year,
                StartUtcDateAndTime = x.StartUtcDateAndTime,
                EndUtcDateAndTime = x.EndUtcDateAndTime,
                HasPlayDateAndTime = x.HasPlayDateAndTime,
            });
            
        var set = new SortedSet<Edition>(resultSet, new EditionDescendingComparer());

        return Task.FromResult(set);
    }
}