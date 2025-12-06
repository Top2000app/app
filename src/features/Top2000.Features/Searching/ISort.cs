namespace Top2000.Features.Searching;

public interface ISort
{
    IOrderedEnumerable<SearchedTrack> Sort(IEnumerable<SearchedTrack> tracks);
}
