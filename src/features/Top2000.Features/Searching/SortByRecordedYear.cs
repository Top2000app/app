namespace Top2000.Features.Searching;

public sealed class SortByRecordedYear : ISort
{
    public IOrderedEnumerable<SearchedTrack> Sort(IEnumerable<SearchedTrack> tracks)
    {
        return tracks.OrderBy(x => x.RecordedYear);
    }
}
