namespace Top2000.Features.Searching;

public sealed class GroupByRecordedYear : IGroup
{
    public IEnumerable<IGrouping<string, SearchedTrack>> Group(IEnumerable<SearchedTrack> tracks)
    {
        return tracks.GroupBy(x => "" + x.RecordedYear);
    }
}
