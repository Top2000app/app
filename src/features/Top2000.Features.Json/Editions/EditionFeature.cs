using Top2000.Data.JsonClientDatabase.Models;
using Top2000.Features.Editions;
using Top2000.Features.Json.Data;
using Edition = Top2000.Features.Editions.Edition;

namespace Top2000.Features.Json.Editions;

public class EditionFeature : IEditions
{
    private readonly DataProvider _dataProvider;

    public EditionFeature(DataProvider dataProvider)
    {
        _dataProvider = dataProvider;
    }
    
    public Task<SortedSet<Edition>> AllEditionsAsync(CancellationToken cancellationToken = default)
    {
        var resultSet = _dataProvider.Value.Editions
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