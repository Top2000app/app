using Top2000.Data.JsonClientDatabase.Models;
using Top2000.Features.Searching;

namespace Top2000.Features.Json.Searching;

public class SearchFeature : ISearch
{
    private readonly Top2000DataContext _data;

    public SearchFeature(Top2000DataContext data)
    {
        _data = data;
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
                Position = _data.Listings
                    .FirstOrDefault(l => l.TrackId == x.Id && l.EditionId == latestYear)?
                    .Position,
                LatestEdition = latestYear,
            });
        
        var sorted = sorting.Sort(toSort);
        var resultSet = group.Group(sorted).ToList();
        
        return Task.FromResult(resultSet);
    }

    private IEnumerable<Track> SearchOnTitleAndArtist(string queryString, int latestYear)
    {
        return _data.Tracks
            .Where(x => x.Artist.Contains(queryString, StringComparison.OrdinalIgnoreCase)
                        || x.Title.Contains(queryString, StringComparison.OrdinalIgnoreCase)
                        || (x.SearchArtist ?? "").Contains(queryString, StringComparison.OrdinalIgnoreCase)
                        || (x.SearchTitle ?? "").Contains(queryString, StringComparison.OrdinalIgnoreCase))
            .Take(100);
    }

    private IEnumerable<Track> SearchOnYear(string queryString, int latestYear, int result)
    {
        return _data.Tracks
            .Where(x => x.RecordedYear == result)
            .Take(100);
    }
}