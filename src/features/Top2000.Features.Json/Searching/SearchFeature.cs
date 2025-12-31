using Top2000.Data.JsonClientDatabase.Models;
using Top2000.Features.Json.Data;
using Top2000.Features.Searching;

namespace Top2000.Features.Json.Searching;

public class SearchFeature : ISearch
{
    private readonly DataProvider _dataProvider;

    public SearchFeature(DataProvider dataProvider)
    {
        _dataProvider = dataProvider;
    }
    
    public Task<List<IGrouping<string, SearchedTrack>>> SearchAsync(string queryString, int latestYear, ISort sorting, IGroup group,
        CancellationToken cancellationToken = default)
    {
        IEnumerable<Track> results = [];

        if (!string.IsNullOrWhiteSpace(queryString))
        {
            results = int.TryParse(queryString, out var year)
                ? SearchOnYear(queryString, latestYear, year)
                : SearchOnTitleAndArtist(queryString, latestYear);
        }
        
        var toSort = results
            .Select(x => new SearchedTrack()
            {
                Id = x.Id,
                Title = x.Title,
                Artist = x.Artist,
                RecordedYear = x.RecordedYear,
                Position = _dataProvider.Value.ListingsOfEdition[latestYear]
                    .FirstOrDefault(l => l.TrackId == x.Id)?
                    .Position,
                LatestEdition = latestYear,
            });
        
        var sorted = sorting.Sort(toSort);
        var resultSet = group.Group(sorted).ToList();
        
        return Task.FromResult(resultSet);
    }
    
    private IEnumerable<Track> SearchOnTitleAndArtist(string queryString, int latestYear)
    {
        return _dataProvider.Value.Tracks.Values
            .Where(x => x.Artist.Contains(queryString, StringComparison.OrdinalIgnoreCase)
                        || x.Title.Contains(queryString, StringComparison.OrdinalIgnoreCase)
                        || (x.SearchArtist ?? "").Contains(queryString, StringComparison.OrdinalIgnoreCase)
                        || (x.SearchTitle ?? "").Contains(queryString, StringComparison.OrdinalIgnoreCase))
            .Take(100);
    }

    private IEnumerable<Track> SearchOnYear(string queryString, int latestYear, int result)
    {
        return _dataProvider.Value.Tracks.Values
            .Where(x => x.RecordedYear == result)
            .Take(100);
    }
}