namespace Top2000.Features.Searching;

public interface ISearch
{
    public Task<List<IGrouping<string, SearchedTrack>>> SearchAsync(string queryString, int latestYear, ISort sorting, IGroup group, CancellationToken cancellationToken = default);
}