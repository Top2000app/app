namespace Top2000.Features.Searching;

public interface IGroup
{
    IEnumerable<IGrouping<string, SearchedTrack>> Group(IEnumerable<SearchedTrack> tracks);
}
