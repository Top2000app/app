using System.Collections.ObjectModel;

namespace Top2000.Features.Searching;

public class SearchTrackRequest : IRequest<List<IGrouping<string, SearchedTrack>>>
{
    public required string QueryString { get; init; }
    public required int LatestYear { get; init; }
    public required ISort Sorting { get; init; }
    public required IGroup Grouping { get; init; }
}