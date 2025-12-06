namespace Top2000.Features.Searching;

public sealed class GroupByArtist : IGroup
{
    public IEnumerable<IGrouping<string, SearchedTrack>> Group(IEnumerable<SearchedTrack> tracks)
        => tracks.GroupBy(x => x.Artist);
}
